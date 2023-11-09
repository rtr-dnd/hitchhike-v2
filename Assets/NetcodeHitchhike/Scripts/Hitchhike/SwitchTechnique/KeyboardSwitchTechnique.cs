using UnityEngine;

public class KeyboardSwitchTechnique : MonoBehaviour, ISwitchTechnique
{
  public int GetFocusedHandAreaIndex(int current)
  {
    int i = current;
    if (!Input.GetKeyDown(KeyCode.Tab)) return i;

    return i >= HitchhikeManager.Instance.handAreaManager.handAreas.Count - 1 ? 0 : i + 1;
  }

}