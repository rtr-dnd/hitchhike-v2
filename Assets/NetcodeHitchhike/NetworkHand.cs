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
        Debug.Log(NetworkManager.Singleton.LocalClientId);

        // if (!IsOwner) return;
        // if (IsServer)
        // {
        //     visual = SpawnDrivenHandOnServer().GetComponent<DrivenHandVisual>();
        // }
        // else
        // {
        //     RequestSpawnDrivenHandOnServerRpc();
        // }
    }

    private NetworkObject SpawnDrivenHandOnServer()
    {
        NetworkObject drivenHand_Network = Instantiate(drivenHandPrefab);
        drivenHand_Network.Spawn();
        return drivenHand_Network;
    }

    [ServerRpc]
    private void RequestSpawnDrivenHandOnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        var visual_Network = SpawnDrivenHandOnServer();
        visual_Network.ChangeOwnership(clientId);
        SetDrivenHandOnClientRpc(visual_Network.NetworkObjectId, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
        });
    }

    [ClientRpc]
    private void SetDrivenHandOnClientRpc(ulong visual_NetworkObjectId, ClientRpcParams clientRpcParams = default)
    {
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
