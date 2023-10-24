using UnityEngine;


public class DisablePlayer : MonoBehaviour
{
  public Camera camera;
  public void DisableScripts()
  {
    camera.enabled = false;
  }
}
