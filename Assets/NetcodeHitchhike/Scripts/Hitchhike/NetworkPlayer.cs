using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    NetworkVariable<ulong> originalHandAreaId = new NetworkVariable<ulong>(ulong.MaxValue);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        originalHandAreaId.OnValueChanged += (previousValue, newValue) => { Debug.Log("original hand area is: " + newValue); };
        if (IsOwner) Invoke(nameof(Displace), 0.5f);
    }
    void Displace()
    {
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = transform.position;
        HitchhikeManager.Instance.handAreaManager.CreateHandArea(new Vector3(
            transform.position.x,
            0.7f,
            transform.position.z + 0.3f
        ), Quaternion.identity, true);
    }
    public void SetOriginalHandArea(ulong id)
    {
        originalHandAreaId.Value = id;
    }
}
