using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HitchhikeMovementPool : SingletonMonoBehaviour<HitchhikeMovementPool>
{
  [HideInInspector] public ReadOnlyHandJointPoses leftJoint;
  [HideInInspector] public ReadOnlyHandJointPoses rightJoint;
}