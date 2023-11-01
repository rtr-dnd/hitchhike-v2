using UnityEngine;
using Unity.Netcode;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHand : NetworkBehaviour
{
    [SerializeField] NetworkObject drivenHandPrefab;
    DrivenHandVisual visual;
    private NetworkVariable<NetworkHandJointPoses> joints = new NetworkVariable<NetworkHandJointPoses>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log(IsServer + " " + IsOwner);
        // if (IsServer)
        // {
        //     visual = SpawnDrivenHandOnServer();
        // }
        // else
        // {
        //     visual = RequestSpawnDrivenHandOnServer();
        // }
    }

    // private DrivenHandVisual SpawnDrivenHandOnServer()
    // {
    //     NetworkObject drivenHand_Network = Instantiate(drivenHandPrefab);
    //     drivenHand_Network.Spawn();
    //     return drivenHand_Network.GetComponent<DrivenHandVisual>();
    // }

    // [ServerRpc]
    // private DrivenHandVisual RequestSpawnDrivenHandOnServer()
    // {
    //     return SpawnDrivenHandOnServer();
    // }

    void Update()
    {
        if (IsOwner)
        {
            if (HitchhikeMovementPool.Instance == null) return;
            if (HitchhikeMovementPool.Instance.leftJoint != null) joints.Value = HitchhikeMovementPool.Instance.leftJoint;
        }
        // if (joints.Value.poses != null && joints.Value.poses.Length != 0) visual.Drive(Pose.identity, joints.Value);
    }
}
