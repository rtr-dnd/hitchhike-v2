using UnityEngine;

public class KeyboardSwitchTechnique : MonoBehaviour, ISwitchTechnique
{
  int m_activeHandAreaIndex = 0;
  public int activeHandAreaIndex => m_activeHandAreaIndex;

  public void UpdateActiveHandAreaIndex(int i)
  {
    m_activeHandAreaIndex = i;
  }

  public int GetFocusedHandAreaIndex()
  {
    int i = activeHandAreaIndex;
    if (!Input.GetKeyDown(KeyCode.Tab)) return i;

    return i >= HitchhikeManager.Instance.handAreaManager.handAreas.Count - 1 ? 0 : i + 1;
  }

}