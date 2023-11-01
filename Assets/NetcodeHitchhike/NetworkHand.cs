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
        if (IsServer)
        {
            visual = SpawnDrivenHandOnServer();
        }
        else
        {
            RequestSpawnDrivenHandOnServerRpc();
        }
    }

    private DrivenHandVisual SpawnDrivenHandOnServer()
    {
        NetworkObject drivenHand_Network = Instantiate(drivenHandPrefab);
        drivenHand_Network.Spawn();
        return drivenHand_Network.GetComponent<DrivenHandVisual>();
    }

    [ServerRpc]
    private void RequestSpawnDrivenHandOnServerRpc()
    {
        visual = SpawnDrivenHandOnServer();
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
