using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class HitchhikeMovementPool : SingletonMonoBehaviour<HitchhikeMovementPool>
{
  [SerializeField, Interface(typeof(IHand))]
  private UnityEngine.Object _syntheticHandLeft;
  public IHand syntheticHandLeft;
  [SerializeField, Interface(typeof(IHand))]
  private UnityEngine.Object _syntheticHandRight;
  public IHand syntheticHandRight;
  public ReadOnlyHandJointPoses leftJoint;
  public ReadOnlyHandJointPoses rightJoint;
  [HideInInspector] public Pose leftPose { get; private set; }
  [HideInInspector] public Pose rightPose { get; private set; }

  protected override void Awake()
  {
    base.Awake();
    syntheticHandLeft = _syntheticHandLeft as IHand;
    syntheticHandRight = _syntheticHandRight as IHand;
  }

  void Update()
  {
    // todo: make it passive (so that this component doesn't have to fetch joints actively)
    syntheticHandLeft.GetJointPosesLocal(out leftJoint);
    syntheticHandRight.GetJointPosesLocal(out rightJoint);
    syntheticHandLeft.GetRootPose(out var pose);
    leftPose = pose;
    syntheticHandRight.GetRootPose(out pose);
    rightPose = pose;
  }
}