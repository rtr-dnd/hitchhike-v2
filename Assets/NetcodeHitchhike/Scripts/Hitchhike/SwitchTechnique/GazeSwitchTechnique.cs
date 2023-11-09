using System.Collections.Generic;
using UnityEngine;

public class GazeSwitchTechnique : MonoBehaviour, ISwitchTechnique
{
  public Transform head;
  public Transform gazeGizmo;
  List<OVREyeGaze> eyeGazes;
  int maxRaycastDistance = 100;
  int m_activeHandAreaIndex = 0;
  public int activeHandAreaIndex => m_activeHandAreaIndex;

  void Awake()
  {
    eyeGazes = new List<OVREyeGaze>(GetComponents<OVREyeGaze>());
  }

  public void UpdateActiveHandAreaIndex(int i)
  {
    m_activeHandAreaIndex = i;
  }

  public int GetFocusedHandAreaIndex()
  {
    int i = activeHandAreaIndex;
    if (Input.GetKeyDown(KeyCode.Tab))
    {
      return i >= HitchhikeManager.Instance.handAreaManager.handAreas.Count - 1 ? 0 : i + 1;
    }

    if (eyeGazes == null) return i;
    if (!eyeGazes[0].EyeTrackingEnabled)
    {
      Debug.Log("Eye tracking not working");
      return i;
    }

    Ray gazeRay = GetGazeRay();
    int layerMask = 1 << LayerMask.NameToLayer("HandArea");

    RaycastHit closestHit = new RaycastHit();
    float closestDistance = float.PositiveInfinity;
    foreach (var hit in Physics.RaycastAll(gazeRay, maxRaycastDistance, layerMask))
    {
      // finding a nearest hit
      var colliderDistance = Vector3.Distance(hit.collider.gameObject.transform.position, head.transform.position);
      if (colliderDistance < closestDistance)
      {
        closestHit = hit;
        closestDistance = colliderDistance;
      }
    }

    HandArea currentGazeArea = null;
    if (closestDistance < float.PositiveInfinity)
    {
      currentGazeArea = GetHandAreaFromHit(closestHit);
      if (currentGazeArea != null)
      {
        i = HitchhikeManager.Instance.handAreaManager.handAreas.FindIndex(area => area == currentGazeArea);
        return i == -1 ? activeHandAreaIndex : i;
      }
    }

    return i;
  }

  private HandArea GetHandAreaFromHit(RaycastHit hit)
  {
    var target = hit.collider.gameObject;
    return target.GetComponent<HandArea>();
  }

  Vector3? filteredDirection = null;
  Vector3? filteredPosition = null;
  float ratio = 0.3f;
  private Ray GetGazeRay()
  {
    Vector3 direction = Vector3.zero;
    eyeGazes.ForEach((e) => { direction += e.transform.forward; });
    direction /= eyeGazes.Count;

    if (!filteredDirection.HasValue)
    {
      filteredDirection = direction;
      filteredPosition = head.transform.position;
    }
    else
    {
      filteredDirection = filteredDirection.Value * (1 - ratio) + direction * ratio;
      filteredPosition = filteredPosition.Value * (1 - ratio) + head.transform.position * ratio;
    }

    if (gazeGizmo != null) gazeGizmo.transform.position = filteredPosition.Value + filteredDirection.Value * 0.5f;
    return new Ray(filteredPosition.Value, filteredDirection.Value);
  }
}
