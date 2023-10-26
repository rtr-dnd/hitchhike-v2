using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HandVisualDriver : MonoBehaviour
{
    [SerializeField, Interface(typeof(IHand))]
    private Object _hand;
    public IHand hand;
    public DrivenHandVisual visual;

    private void Awake()
    {
        hand = _hand as IHand;
    }

    void Update()
    {
        hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints);
        if (localJoints == null) return;
        visual.Drive(Pose.identity, localJoints);
    }
}
