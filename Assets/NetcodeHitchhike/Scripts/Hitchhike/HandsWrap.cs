using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using System.Linq;

public class HandsWrap : MonoBehaviour
{
    float m_handVisualScale = 1f;
    public float handVisualScale
    {
        get { return m_handVisualScale; }
        set
        {
            m_handVisualScale = value;
            if (displacedHands == null) return;
            foreach (var hand in displacedHands) { hand.scale = value; }
        }
    }
    bool m_frozen = false;
    public bool frozen
    {
        get { return m_frozen; }
        set
        {
            m_frozen = value;
            if (displacedHands == null) return;
            foreach (var hand in displacedHands) { hand.frozen = value; }
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

    List<DisplacedHand> displacedHands;

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
        displacedHands = GetComponentsInChildren<DisplacedHand>().ToList();

        // initialize value
        foreach (var hand in displacedHands)
        {
            if (coordinate != null) hand.thisSpace = coordinate.transform;
            if (originalCoordinate != null) hand.originalSpace = originalCoordinate.transform;
            hand.scale = handVisualScale;
            hand.frozen = frozen;
        }
    }

    // void Update()
    // {
    //     if (frozen) return;
    //     leftFinalHand.GetJointPosesLocal(out HitchhikeMovementPool.Instance.leftJoint);
    //     rightFinalHand.GetJointPosesLocal(out HitchhikeMovementPool.Instance.rightJoint);
    // }

    void OnCoordinateChanged()
    {
        if (originalCoordinate == null || coordinate == null) return;
        if (displacedHands == null) return;

        foreach (var hand in displacedHands)
        {
            hand.thisSpace = coordinate.transform;
            hand.originalSpace = originalCoordinate.transform;
        }

        handVisualScale = new float[] {
            coordinate.transform.lossyScale.x / originalCoordinate.transform.lossyScale.x,
            coordinate.transform.lossyScale.y / originalCoordinate.transform.lossyScale.y,
            coordinate.transform.lossyScale.z / originalCoordinate.transform.lossyScale.z
        }.Average();
    }
}