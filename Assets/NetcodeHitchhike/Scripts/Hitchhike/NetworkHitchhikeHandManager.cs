using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Utilities;
using System.Data.Common;
using System.Linq;
using Oculus.Interaction;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHitchhikeHandManager : NetworkBehaviour
{
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
    // pose of active synthetic hand
    private NetworkVariable<NetworkPose> leftPose = new NetworkVariable<NetworkPose>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<NetworkPose> rightPose = new NetworkVariable<NetworkPose>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    // defaults to MaxValue
    private NetworkVariable<ulong> leftVisualNetworkId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<ulong> rightVisualNetworkId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // sync visuals to networkid
        leftVisualNetworkId.OnValueChanged += (previous, current) => StartCoroutine(SetHandNetworkVisualLoop(current, Handedness.Left));
        rightVisualNetworkId.OnValueChanged += (previous, current) => StartCoroutine(SetHandNetworkVisualLoop(current, Handedness.Right));

        if (!IsServer) StartCoroutine(CheckHandNetworkIdLoop());

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
        var hasInitialized = (leftVisualNetworkId.Value != ulong.MaxValue) && (rightVisualNetworkId.Value != ulong.MaxValue);
        if (hasInitialized)
        {
            StartCoroutine(SetHandNetworkVisualLoop(leftVisualNetworkId.Value, Handedness.Left));
            StartCoroutine(SetHandNetworkVisualLoop(rightVisualNetworkId.Value, Handedness.Right));
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckHandNetworkIdLoop());
    }

    IEnumerator SetHandNetworkVisualLoop(ulong current, Handedness handedness)
    {
        if (current == ulong.MaxValue) yield break;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(current))
        {
            // found spawned hand on local env
            if (handedness == Handedness.Left)
            {
                leftVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[current].GetComponent<DrivenHandVisual>();
                if (IsOwner) leftVisual.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            if (handedness == Handedness.Right)
            {
                rightVisual = NetworkManager.Singleton.SpawnManager.SpawnedObjects[current].GetComponent<DrivenHandVisual>();
                if (IsOwner) rightVisual.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetHandNetworkVisualLoop(current, handedness));
    }

    // server creates handvisual
    private void SpawnDrivenHandOnServer(ulong ownerClientId, Handedness handedness)
    {
        NetworkObject drivenHand_Network = Instantiate(handedness == Handedness.Left ? drivenHandPrefabLeft : drivenHandPrefabRight);
        drivenHand_Network.SpawnWithOwnership(ownerClientId);
        if (handedness == Handedness.Left)
        {
            leftVisualNetworkId.Value = drivenHand_Network.NetworkObjectId;
        }
        else
        {
            rightVisualNetworkId.Value = drivenHand_Network.NetworkObjectId;
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
            // if (HitchhikeMovementPool.Instance == null) return;
            // if (HitchhikeMovementPool.Instance.leftJoint != null) leftJoints.Value = HitchhikeMovementPool.Instance.leftJoint;
            // if (HitchhikeMovementPool.Instance.rightJoint != null) rightJoints.Value = HitchhikeMovementPool.Instance.rightJoint;
        }
        if (leftVisual != null && leftJoints.Value.poses != null && leftJoints.Value.poses.Length != 0) leftVisual.Drive(Pose.identity, leftJoints.Value);
        if (rightVisual != null && rightJoints.Value.poses != null && rightJoints.Value.poses.Length != 0) rightVisual.Drive(Pose.identity, rightJoints.Value);
        if (leftVisual != null && leftPose != null) leftVisual.transform.SetPose(leftPose.Value);
        if (rightVisual != null && rightPose != null) rightVisual.transform.SetPose(rightPose.Value);
    }
}
