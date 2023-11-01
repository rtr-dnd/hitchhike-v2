using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HitchhikeMovementPool : SingletonMonoBehaviour<HitchhikeMovementPool>
{
  [SerializeField, Interface(typeof(IHand))]
  private UnityEngine.Object _rawHand;
  public IHand rawHand;
  public ReadOnlyHandJointPoses leftJoint;

  protected override void Awake()
  {
    base.Awake();
    rawHand = _rawHand as IHand;
  }

  void Update()
  {
    rawHand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints);
    leftJoint = localJoints;
  }
}