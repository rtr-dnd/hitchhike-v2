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

    // SETUP
    // 0. copy OVRRight(Left)HandVisual
    // 1. assign NetworkObject
    // 2. assign this
    // 3. push "Auto Map Joints"
    // 4. remove HandVisual

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
    public void SetScale(float scale)
    {
        if (_root != null) _root.transform.localScale = new Vector3(scale, scale, scale);
    }
}
