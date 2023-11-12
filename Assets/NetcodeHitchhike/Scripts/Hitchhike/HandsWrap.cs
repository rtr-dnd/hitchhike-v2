using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using System.Linq;
using Oculus.Interaction.HandGrab;
using System.Collections;

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

    void Awake()
    {
        leftFinalHand = _leftFinalHand as IHand;
        rightFinalHand = _rightFinalHand as IHand;
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

    public HandGrabInteractable GetCurrentInteractable(Handedness handedness)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        return hand.GetComponentInChildren<HandGrabInteractor>().SelectedInteractable;
    }

    public void Select(Handedness handedness, HandGrabInteractable interactable, HandGrabTarget target)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        var interactor = hand.GetComponentInChildren<HandGrabInteractor>();
        switch (target.Anchor)
        {
            case HandGrabTarget.GrabAnchor.Pinch:
                interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.Pinch;
                break;
            case HandGrabTarget.GrabAnchor.Palm:
                interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.Palm;
                break;
        }
        interactor.HandGrabTarget.Set(null, target.HandAlignment, target.Anchor, target._handGrabResult);
        interactor.ForceSelect(interactable, true);
        StartCoroutine(ResetGrabOverride(interactor));
    }

    IEnumerator ResetGrabOverride(HandGrabInteractor interactor)
    {
        yield return new WaitForSeconds(0.5f);
        interactor.grabTypeOverride = Oculus.Interaction.Grab.GrabTypeFlags.None;
    }

    public HandGrabTarget Unselect(Handedness handedness)
    {
        var hand = handedness == Handedness.Left ? leftHand : rightHand;
        var interactor = hand.GetComponentInChildren<HandGrabInteractor>();
        var target = interactor.HandGrabTarget;
        interactor.Unselect();
        var grabUse = hand.GetComponentInChildren<HandGrabUseInteractor>();
        if (grabUse != null) grabUse.Unselect();
        return target;
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
