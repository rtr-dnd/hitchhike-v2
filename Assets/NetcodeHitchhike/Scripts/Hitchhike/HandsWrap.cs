using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using System.Linq;

public class HandsWrap : MonoBehaviour
{
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
            hand.frozen = frozen;
        }
    }

    void OnCoordinateChanged()
    {
        if (originalCoordinate == null || coordinate == null) return;
        if (displacedHands == null) return;

        foreach (var hand in displacedHands)
        {
            hand.thisSpace = coordinate.transform;
            hand.originalSpace = originalCoordinate.transform;
        }
    }
}
