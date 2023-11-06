using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using System.Linq;

public class HandsWrap : MonoBehaviour
{
    Vector3 m_disposition = Vector3.zero;
    public Vector3 disposition
    {
        get { return m_disposition; }
        set
        {
            m_disposition = value;
            if (displacedHands == null) return;
            foreach (var hand in displacedHands) { hand.disposition = value; }
        }
    }
    float m_scale = 1f;
    public float scale
    {
        get { return m_scale; }
        set
        {
            m_scale = value;
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

    void Start()
    {
        displacedHands = GetComponentsInChildren<DisplacedHand>().ToList();

        // initialize value
        foreach (var hand in displacedHands)
        {
            hand.disposition = disposition;
            hand.scale = scale;
            hand.frozen = frozen;
        }
    }

    void OnCoordinateChanged()
    {
        if (originalCoordinate == null || coordinate == null) return;
        disposition = coordinate.transform.position - originalCoordinate.transform.position;
        scale = new float[] {
            coordinate.transform.lossyScale.x / originalCoordinate.transform.lossyScale.x,
            coordinate.transform.lossyScale.y / originalCoordinate.transform.lossyScale.y,
            coordinate.transform.lossyScale.z / originalCoordinate.transform.lossyScale.z
        }.Average();
    }
}
