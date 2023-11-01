using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HitchhikeMovementPool : SingletonMonoBehaviour<HitchhikeMovementPool>
{
  [SerializeField, Interface(typeof(IHand))]
  private UnityEngine.Object _rawHandLeft;
  public IHand rawHandLeft;
  [SerializeField, Interface(typeof(IHand))]
  private UnityEngine.Object _rawHandRight;
  public IHand rawHandRight;
  public ReadOnlyHandJointPoses leftJoint;
  public ReadOnlyHandJointPoses rightJoint;

  protected override void Awake()
  {
    base.Awake();
    rawHandLeft = _rawHandLeft as IHand;
    rawHandRight = _rawHandRight as IHand;
  }

  void Update()
  {
    // todo: make it passive (so that this component doesn't have to fetch joints actively)
    rawHandLeft.GetJointPosesLocal(out leftJoint);
    rawHandRight.GetJointPosesLocal(out rightJoint);
  }
}