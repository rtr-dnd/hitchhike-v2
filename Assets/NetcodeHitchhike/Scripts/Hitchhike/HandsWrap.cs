using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using System.Linq;
using Oculus.Interaction.HandGrab;
using System.Collections;
using Oculus.Interaction;
//using RootScript;
using Oculus.Interaction.Grab;
using Hitchhike;

public class HandsWrap : MonoBehaviour
{
    bool m_frozen = false;
    public bool frozen
    {
        get { return m_frozen; }
        set
        {
            m_frozen = value;
            if (displacedHandLeft == null || displacedHandRight == null) return;
            displacedHandLeft.frozen = value;
            displacedHandRight.frozen = value;
        }
    }
    HandAreaCoordinate m_originalCoordinate;
    public HandAreaCoordinate originalCoordinate
    {
        get { return m_originalCoordinate; }
        set
        {
            m_originalCoordinate = value;
            OnCoordinateChanged();
        }
    }
    HandAreaCoordinate m_coordinate;
    public HandAreaCoordinate coordinate
    {
        get { return m_coordinate; }
        set
        {
            m_coordinate = value;
            OnCoordinateChanged();
        }
    }

    DisplacedHand displacedHandLeft;
    DisplacedHand displacedHandRight;


    public GameObject leftHand;
    public GameObject rightHand;
    [SerializeField, InterfaceType(typeof(IHand))]
    private UnityEngine.Object _leftFinalHand;
    public IHand leftFinalHand;
    [SerializeField, InterfaceType(typeof(IHand))]
    private UnityEngine.Object _rightFinalHand;
    public IHand rightFinalHand;
    //private HitchhikeHandGrabInteractor grab;

    void Awake()
    {
        leftFinalHand = _leftFinalHand as IHand;
        rightFinalHand = _rightFinalHand as IHand;
        //grab = gameObject.GetComponentInChildren<HitchhikeHandGrabInteractor>();
    }
    void Start()
    {
        displacedHandLeft = leftHand.GetComponentInChildren<DisplacedHand>();
        displacedHandRight = rightHand.GetComponentInChildren<DisplacedHand>();

        // initialize value
        foreach (var hand in new List<DisplacedHand>() { displacedHandLeft, displacedHandRight })
        {
            if (coordinate != null) hand.thisSpace = coordinate.transform;
            if (originalCoordinate != null) hand.originalSpace = originalCoordinate.transform;
            hand.frozen = frozen;
        }
    }

    void Update()
    {
        if (frozen) return;
        var hand = rightHand;
        var interactor = hand.GetComponentInChildren<HitchhikeHandGrabInteractor>();
        var target = interactor.HandGrabTarget;
        //var result = target._handGrabResult;
        //Debug.Log("HandGrabResult RelativePose: " + result.RelativePose);
    }

    public HandGrabInteractable GetCurrentInteractable(Handedness handedness)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        return hand.GetComponentInChildren<HitchhikeHandGrabInteractor>().SelectedInteractable;
    }

    // public void Select(Handedness handedness, HandGrabInteractable interactable, HandGrabTarget target)
    // {
    //     var hand = handedness == Handedness.Left ? leftHand : rightHand;
    //     var interactor = hand.GetComponentInChildren<HandGrabInteractor>();
    //     switch (target.Anchor)
    //     {
    //         case HandGrabTarget.GrabAnchor.Pinch:
    //             interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.Pinch;
    //             break;
    //         case HandGrabTarget.GrabAnchor.Palm:
    //             interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.Palm;
    //             break;
    //     }
    //     interactor.HandGrabTarget.Set(null, target.HandAlignment, target.Anchor, target._handGrabResult);
    //     interactor.ForceSelect(interactable, true);
    //     StartCoroutine(ResetGrabOverride(interactor));
    // }

    // public void Select(Handedness handedness, HandGrabInteractable interactable, HandGrabTarget target)
    // {
    //     var hand = handedness == Handedness.Left ? leftHand : rightHand;
    //     var interactor = hand.GetComponentInChildren<HandGrabInteractor>();
    //     //var result = target._handGrabResult;
    //     //Debug.Log("HandGrabTarget RelativeTo: " + target._relativeTo);
    //     //Debug.Log("HandGrabResult RelativePose2: " + result.RelativePose);
    //     //var result = target._handGrabResult;

    //     interactor.ForceSelect(interactable, true);
    //     //interactor.HandGrabTarget.Set(null, target.HandAlignment, target.Anchor, target._handGrabResult);
    //     //StartCoroutine(ResetGrabOverride(interactor));
    // }
    public class SavedGrabState
    {
      public HandGrabTarget target;
      public Pose relativePose;
      public Vector3 objectScale;
    }

    public void Select(Handedness handedness, HandGrabInteractable interactable, SavedGrabState savedState)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        var grab = hand.GetComponentInChildren<HitchhikeHandGrabInteractor>();
        var newResult = new HandGrabResult();

        // Copy HandPose if it exists
        if (savedState.target.HandPose != null)
        {
            newResult.HasHandPose = true;
            newResult.HandPose.CopyFrom(savedState.target.HandPose);
        }

        // Compensate for scale changes between Unselect and Select
        Vector3 currentScale = interactable.RelativeTo.lossyScale;
        Vector3 scaleRatio = new Vector3(
            currentScale.x / savedState.objectScale.x,
            currentScale.y / savedState.objectScale.y,
            currentScale.z / savedState.objectScale.z
        );

        // Adjust the relative pose to maintain the same world-space grab point
        // When object scales down, the relative offset should scale up proportionally
        Vector3 compensatedPosition = new Vector3(
            savedState.relativePose.position.x * scaleRatio.x,
            savedState.relativePose.position.y * scaleRatio.y,
            savedState.relativePose.position.z * scaleRatio.z
        );

        newResult.RelativePose = new Pose(compensatedPosition, savedState.relativePose.rotation);

        // Force select with the custom target
        grab.ForceSelectWithCustomTarget(
            interactable,
            newResult,
            savedState.target.Anchor,
            savedState.target.HandAlignment
      );
    }
    

    IEnumerator ResetGrabOverride(HitchhikeHandGrabInteractor interactor)
    {
        yield return new WaitForSeconds(0.5f);
        //interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.None;
    }

    // public HandGrabTarget Unselect(Handedness handedness)
    // {
    //     var hand = handedness == Handedness.Left ? leftHand : rightHand;
    //     var interactor = hand.GetComponentInChildren<HitchhikeHandGrabInteractor>();
    //     var target = interactor.HandGrabTarget;
    //     interactor.Unselect();
    //     var grabUse = hand.GetComponentInChildren<HandGrabUseInteractor>();
    //     if (grabUse != null) grabUse.Unselect();
    //     return target;
    // }

    public SavedGrabState Unselect(Handedness handedness)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        var grab = hand.GetComponentInChildren<HitchhikeHandGrabInteractor>();
        var grabUse = hand.GetComponentInChildren<HandGrabUseInteractor>();
        if (grab.SelectedInteractable == null)
        {
            grab.Unselect();
            if (grabUse != null) grabUse.Unselect();
            return null;
        }

        var state = new SavedGrabState();
        state.target = grab.HandGrabTarget;

        // Get current grab point in world space
        Pose worldGrabPose = grab.HandGrabTarget.GetWorldPoseDisplaced(Pose.identity);
        Transform relativeTo = grab.SelectedInteractable.RelativeTo;

        // Save the object's current scale for later compensation
        state.objectScale = relativeTo.lossyScale;

        // Convert to relative pose (ignoring scale to handle objects with non-uniform scale)
        Vector3 worldOffset = worldGrabPose.position - relativeTo.position;
        Vector3 localOffset = Quaternion.Inverse(relativeTo.rotation) * worldOffset;
        Quaternion localRotation = Quaternion.Inverse(relativeTo.rotation) * worldGrabPose.rotation;

        state.relativePose = new Pose(localOffset, localRotation);

        grab.Unselect();
        if (grabUse != null) grabUse.Unselect();

        return state;
    }

    void OnCoordinateChanged()
    {
        if (originalCoordinate == null || coordinate == null) return;
        if (displacedHandLeft == null || displacedHandRight == null) return;

        foreach (var hand in new List<DisplacedHand>() { displacedHandLeft, displacedHandRight })
        {
            hand.thisSpace = coordinate.transform;
            hand.originalSpace = originalCoordinate.transform;
        }
    }
}
