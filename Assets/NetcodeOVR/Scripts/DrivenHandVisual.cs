using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Unity.Netcode;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class DrivenHandVisual : NetworkBehaviour
{
    // for editor joint assigning only
    // todo: remove this dependency
    [SerializeField, Interface(typeof(IHand))]
    private Object _hand;
    public IHand hand;
    [SerializeField, Optional]
    private Transform _root = null;

    [HideInInspector]
    [SerializeField]
    private List<Transform> _jointTransforms = new List<Transform>();
    public IList<Transform> Joints => _jointTransforms;

    private void Awake()
    {
        if (_root == null && _jointTransforms.Count > 0 && _jointTransforms[0] != null)
        {
            _root = _jointTransforms[0].parent;
        }
    }

    // todo: use rootpose
    // todo: return if localJoints don't match
    public void Drive(Pose rootPose, ReadOnlyHandJointPoses localJoints)
    {
        if (localJoints.Count != Constants.NUM_HAND_JOINTS) return;
        for (var i = 0; i < Constants.NUM_HAND_JOINTS; ++i)
        {
            if (_jointTransforms[i] == null)
            {
                continue;
            }
            _jointTransforms[i].SetPose(localJoints[i], Space.Self);
        }
    }
}
