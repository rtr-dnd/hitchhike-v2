using System.Collections.Generic;
using UnityEngine;

public class GazeSwitchTechnique : MonoBehaviour, ISwitchTechnique
{
  public Transform head;
  public Transform gazeGizmo;
  private Ray gazeRay;
  private Ray frontRay;
  List<OVREyeGaze> eyeGazes;
  int maxRaycastDistance = 100;
  [SerializeField] private bool useGaze = true;
  [SerializeField] private bool DebugMode = true;

  void Awake()
  {
    eyeGazes = new List<OVREyeGaze>(GetComponents<OVREyeGaze>());
  }

  public int GetFocusedHandAreaIndex(int current)
  {
    int i = current;
    if (Input.GetKeyDown(KeyCode.Tab))
    {
      return i >= LocalHitchhikeManager.Instance.handAreaManager.handAreas.Count - 1 ? 0 : i + 1;
    }

    frontRay = GetFrontRay();

    
    if (eyeGazes == null) return i;
    if (!eyeGazes[0].EyeTrackingEnabled)
    {
      Debug.Log("Eye tracking not working");
      return i;
    }
    gazeRay = GetGazeRay();
    

    
    Ray areaRay = useGaze ? gazeRay : frontRay;

    int layerMask = 1 << LayerMask.NameToLayer("HandArea");

    RaycastHit closestHit = new RaycastHit();
    float closestDistance = float.PositiveInfinity;
    foreach (var hit in Physics.RaycastAll(areaRay, maxRaycastDistance, layerMask))
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
        i = LocalHitchhikeManager.Instance.handAreaManager.handAreas.FindIndex(area => area == currentGazeArea);
        return i == -1 ? current : i;
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
    //Debug.DrawRay(filteredPosition.Value, filteredDirection.Value*10f, Color.blue);
    return new Ray(filteredPosition.Value, filteredDirection.Value);
  }

  private Ray GetFrontRay()
  {
    Vector3 direction = Quaternion.AngleAxis(8f, head.transform.right) * head.transform.forward; 
    //Debug.DrawRay(head.transform.position, direction, Color.red);
    return new Ray(head.transform.position, direction);
  }

  void OnDrawGizmos(){
    if (!DebugMode) return;
    //Debug.DrawRay(ray.origin, ray.direction*10f, Color.green);
    if (Physics.Raycast(gazeRay, out RaycastHit hit))
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * hit.distance);

        // 2. ヒットした場所(hit.point)に赤い丸を表示
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.01f); // 第2引数は半径
    }
    else
    {
        // ヒットしなかった場合のRay（緑色）
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * 100f);
    }
    if (Physics.Raycast(frontRay, out RaycastHit hit2))
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(frontRay.origin, frontRay.direction * hit2.distance);

        // 2. ヒットした場所(hit2.point)に赤い丸を表示
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit2.point, 0.01f); // 第2引数は半径
    }
    else
    {
        // ヒットしなかった場合のRay（緑色）
        Gizmos.color = Color.green;
        Gizmos.DrawRay(frontRay.origin, frontRay.direction * 100f);
    }

  }
}
