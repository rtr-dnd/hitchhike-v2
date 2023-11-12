using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovementPool : NetworkBehaviour
{
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
}
