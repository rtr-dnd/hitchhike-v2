using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction;

public class PlayerMovementPool : NetworkBehaviour
{
    public GameObject hmdPrefab;
    GameObject headVisual; // todo: divide into dedicated script
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
    public NetworkVariable<NetworkPose> hmdPosePool = new NetworkVariable<NetworkPose>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        headVisual = Instantiate(hmdPrefab);
    }

    private void Update()
    {
        headVisual.transform.SetPose(hmdPosePool.Value);
    }
}
