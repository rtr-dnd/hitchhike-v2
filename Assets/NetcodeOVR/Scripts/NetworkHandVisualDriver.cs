using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Linq;
using System;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHandVisualDriver : NetworkBehaviour
{
    [SerializeField, Interface(typeof(IHand))]
    private UnityEngine.Object _hand;
    public IHand hand;
    public DrivenHandVisual visual;
    private NetworkVariable<NetworkHandJointPoses> joints = new NetworkVariable<NetworkHandJointPoses>();

    private void Awake()
    {
        hand = _hand as IHand;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Update()
    {
        if (IsHost)
        {
            hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints);
            if (localJoints == null) return;
            joints.Value = localJoints;
        }
        if (joints.Value.poses != null && joints.Value.poses.Length != 0) visual.Drive(Pose.identity, joints.Value);
    }
}
