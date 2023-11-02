using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Data.Common;

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
    // defaults to MaxValue
    private NetworkVariable<ulong> leftNetworkId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<ulong> rightNetworkId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // sync visuals to networkid
        leftNetworkId.OnValueChanged += (previous, current) => SetHandNetworkVisual(current, Handedness.Left);
        rightNetworkId.OnValueChanged += (previous, current) => SetHandNetworkVisual(current, Handedness.Right);

        if (!IsServer) StartCoroutine("CheckHandNetworkIdLoop");

        // hand without ownership (passive)
        if (!IsOwner) return;

        // server side spawning hand with ownership (active)
        if (IsServer)
        {
            SpawnDrivenHandOnServer(NetworkManager.LocalClientId, Handedness.Left);
            SpawnDrivenHandOnServer(NetworkManager.LocalClientId, Handedness.Right);
            return;
        }

        // client side spawning hand with ownership (active)
        RequestSpawnDrivenHandOnServerRpc(Handedness.Left);
        RequestSpawnDrivenHandOnServerRpc(Handedness.Right);
    }

    IEnumerator CheckHandNetworkIdLoop()
    {
        var hasInitialized = leftNetworkId.Value != ulong.MaxValue && rightNetworkId.Value != ulong.MaxValue;
        if (hasInitialized)
        {
            SetHandNetworkVisual(leftNetworkId.Value, Handedness.Left);
            SetHandNetworkVisual(rightNetworkId.Value, Handedness.Right);
            yield break;
        }
        yield return new WaitForSeconds(1);
        StartCoroutine("CheckHandNetworkIdLoop");
    }

    private void SetHandNetworkVisual(ulong current, Handedness handedness)
    {
        if (current == ulong.MaxValue) return;
        if (handedness == Handedness.Left)
        {
            leftVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[current].GetComponent<DrivenHandVisual>();
        }
        else
        {
            rightVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[current].GetComponent<DrivenHandVisual>();
        }
    }

    // server creates handvisual
    private void SpawnDrivenHandOnServer(ulong ownerClientId, Handedness handedness)
    {
        NetworkObject drivenHand_Network = Instantiate(handedness == Handedness.Left ? drivenHandPrefabLeft : drivenHandPrefabRight);
        drivenHand_Network.SpawnWithOwnership(ownerClientId);
        if (handedness == Handedness.Left)
        {
            leftNetworkId.Value = drivenHand_Network.NetworkObjectId;
        }
        else
        {
            rightNetworkId.Value = drivenHand_Network.NetworkObjectId;
        }
    }

    // request creation of handvisual to server
    [ServerRpc]
    private void RequestSpawnDrivenHandOnServerRpc(Handedness handedness, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        SpawnDrivenHandOnServer(clientId, handedness);
    }

    void Update()
    {
        if (!IsSpawned) return;
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
