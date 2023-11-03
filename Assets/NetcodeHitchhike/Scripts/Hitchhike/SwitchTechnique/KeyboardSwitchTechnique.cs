using UnityEngine;

public class KeyboardSwitchTechnique : MonoBehaviour, ISwitchTechnique
{
  int m_activeHandAreaIndex = 0;
  public int activeHandAreaIndex => m_activeHandAreaIndex;

  public int GetFocusedHandAreaIndex()
  {
    int i = activeHandAreaIndex;
    if (!Input.GetKeyDown(KeyCode.Tab)) return i;

    return i >= HitchhikeManager.Instance.handAreas.Count - 1 ? 0 : i + 1;
  }

  public void UpdateActiveHandAreaIndex(int i)
  {
    m_activeHandAreaIndex = i;
  }
}