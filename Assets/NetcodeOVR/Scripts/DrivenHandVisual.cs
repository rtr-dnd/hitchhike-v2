using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class DrivenHandVisual : MonoBehaviour
{
    // for editor joint assigning only
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

    public void Drive(Pose rootPose, ReadOnlyHandJointPoses localJoints)
    {
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
