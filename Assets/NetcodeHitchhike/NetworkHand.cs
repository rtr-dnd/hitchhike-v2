using UnityEngine;
using Unity.Netcode;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHand : NetworkBehaviour
{
    [SerializeField] NetworkObject drivenHandPrefab;
    DrivenHandVisual visual;
    private NetworkVariable<NetworkHandJointPoses> joints = new NetworkVariable<NetworkHandJointPoses>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;
        if (IsServer)
        {
            SpawnDrivenHandOnServer(NetworkManager.LocalClientId);
        }
        else
        {
            RequestSpawnDrivenHandOnServerRpc();
        }
    }

    private void SpawnDrivenHandOnServer(ulong ownerClientId)
    {
        NetworkObject drivenHand_Network = Instantiate(drivenHandPrefab);
        drivenHand_Network.SpawnWithOwnership(ownerClientId);
        SetDrivenHandOnClientRpc(this.NetworkObjectId, drivenHand_Network.NetworkObjectId);
    }

    [ServerRpc]
    private void RequestSpawnDrivenHandOnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        SpawnDrivenHandOnServer(clientId);
    }

    [ClientRpc]
    private void SetDrivenHandOnClientRpc(ulong hand_NetworkObjectId, ulong visual_NetworkObjectId)
    {
        if (hand_NetworkObjectId != this.NetworkObjectId) return;
        visual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[visual_NetworkObjectId].GetComponent<DrivenHandVisual>();
    }

    void Update()
    {
        if (IsOwner)
        {
            if (HitchhikeMovementPool.Instance == null) return;
            if (HitchhikeMovementPool.Instance.leftJoint != null) joints.Value = HitchhikeMovementPool.Instance.leftJoint;
        }
        Debug.Log(visual);
        // if (joints.Value.poses != null && joints.Value.poses.Length != 0) visual.Drive(Pose.identity, joints.Value);
    }
}
