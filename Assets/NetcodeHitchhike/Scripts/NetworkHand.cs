using UnityEngine;
using Unity.Netcode;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHand : NetworkBehaviour
{
    enum Handedness
    {
        Left,
        Right
    }
    [SerializeField] NetworkObject drivenHandPrefabLeft;
    [SerializeField] NetworkObject drivenHandPrefabRight;
    DrivenHandVisual leftVisual;
    DrivenHandVisual rightVisual;
    private NetworkVariable<NetworkHandJointPoses> leftJoints = new NetworkVariable<NetworkHandJointPoses>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<NetworkHandJointPoses> rightJoints = new NetworkVariable<NetworkHandJointPoses>(
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
            SpawnDrivenHandOnServer(NetworkManager.LocalClientId, Handedness.Left);
            SpawnDrivenHandOnServer(NetworkManager.LocalClientId, Handedness.Right);
        }
        else
        {
            RequestSpawnDrivenHandOnServerRpc(Handedness.Left);
            RequestSpawnDrivenHandOnServerRpc(Handedness.Right);
        }
    }

    // server creates handvisual and notifies all clients
    private void SpawnDrivenHandOnServer(ulong ownerClientId, Handedness handedness)
    {
        NetworkObject drivenHand_Network = Instantiate(handedness == Handedness.Left ? drivenHandPrefabLeft : drivenHandPrefabRight);
        drivenHand_Network.SpawnWithOwnership(ownerClientId);
        SetDrivenHandOnClientRpc(handedness, this.NetworkObjectId, drivenHand_Network.NetworkObjectId);
    }

    // request creation of handvisual to server
    [ServerRpc]
    private void RequestSpawnDrivenHandOnServerRpc(Handedness handedness, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        SpawnDrivenHandOnServer(clientId, handedness);
    }

    // broadcast ID of the created handvisual to the same NetworkHand on all clients
    // client registers the hand visual for later use
    [ClientRpc]
    private void SetDrivenHandOnClientRpc(Handedness handedness, ulong hand_NetworkObjectId, ulong visual_NetworkObjectId)
    {
        if (hand_NetworkObjectId != this.NetworkObjectId) return;
        if (handedness == Handedness.Left)
        {
            leftVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[visual_NetworkObjectId].GetComponent<DrivenHandVisual>();
        }
        else
        {
            rightVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[visual_NetworkObjectId].GetComponent<DrivenHandVisual>();
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            if (HitchhikeMovementPool.Instance == null) return;
            if (HitchhikeMovementPool.Instance.leftJoint != null) leftJoints.Value = HitchhikeMovementPool.Instance.leftJoint;
            if (HitchhikeMovementPool.Instance.rightJoint != null) rightJoints.Value = HitchhikeMovementPool.Instance.rightJoint;
        }
        if (leftVisual != null && leftJoints.Value.poses != null && leftJoints.Value.poses.Length != 0) leftVisual.Drive(Pose.identity, leftJoints.Value);
        if (rightVisual != null && rightJoints.Value.poses != null && rightJoints.Value.poses.Length != 0) rightVisual.Drive(Pose.identity, rightJoints.Value);
        Debug.Log(rightJoints.Value.poses);
        Debug.Log(rightVisual);
    }
}
