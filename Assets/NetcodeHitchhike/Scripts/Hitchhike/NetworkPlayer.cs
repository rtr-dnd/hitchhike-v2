using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    NetworkVariable<ulong> originalHandAreaId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<ulong> activeHandAreaId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<NetworkHandJointPoses> leftJointsPool = new NetworkVariable<NetworkHandJointPoses>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<NetworkHandJointPoses> rightJointsPool = new NetworkVariable<NetworkHandJointPoses>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        originalHandAreaId.OnValueChanged += (previousValue, newValue) =>
        {
            Debug.Log("original hand area is: " + newValue);
            HitchhikeManager.Instance.handAreaManager.handAreas.ForEach(area =>
                area.GetCoordinateForClient(this.OwnerClientId).SetOriginalCoordinate(PlayerOriginalCoordinate()));
            if (!IsOwner) return;
            activeHandAreaId.Value = newValue;
        };
        activeHandAreaId.OnValueChanged += (previousValue, newValue) =>
        {
            if (!IsOwner) return;
            if (newValue == ulong.MaxValue) return;
            if (HitchhikeManager.Instance.handAreaManager.handAreas == null) return;
            // todo: startcoroutine
            var i = HitchhikeManager.Instance.handAreaManager.handAreas.FindIndex(area => area.GetComponent<NetworkObject>().NetworkObjectId == newValue);
            if (i == -1) return;
            foreach (var item in HitchhikeManager.Instance.handAreaManager.handAreas.Select((area, index) => new { area, index }))
            {
                item.area.GetCoordinateForClient(this.OwnerClientId).isEnabled = item.index == i;
            }
        };
        if (IsOwner) Invoke(nameof(DisplacePlayer), 0.5f);
    }
    void DisplacePlayer()
    {
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = transform.position;
        HitchhikeManager.Instance.handAreaManager.CreateHandArea(new Vector3(
            transform.position.x,
            0.7f,
            transform.position.z + 0.3f
        ), Quaternion.identity, NetworkManager.LocalClientId);
    }
    public void SetOriginalHandArea(ulong id)
    {
        if (!IsOwner) return;
        originalHandAreaId.Value = id;
    }
    public HandAreaCoordinate PlayerOriginalCoordinate()
    {
        if (originalHandAreaId == null || originalHandAreaId.Value == ulong.MaxValue) return null;
        return NetworkManager.SpawnManager.SpawnedObjects[originalHandAreaId.Value].GetComponent<HandArea>().GetCoordinateForClient(this.OwnerClientId);
    }

    void Update()
    {
        if (!IsOwner) return;
        var newActiveHandAreaIndex = HitchhikeManager.Instance.switchTechnique.GetFocusedHandAreaIndex();
        var newActiveId = HitchhikeManager.Instance.handAreaManager.handAreas[newActiveHandAreaIndex].GetComponent<NetworkObject>().NetworkObjectId;
        if (activeHandAreaId.Value != newActiveId)
        {
            activeHandAreaId.Value = newActiveId;
            HitchhikeManager.Instance.switchTechnique.UpdateActiveHandAreaIndex(newActiveHandAreaIndex);
        }
    }
}
