public interface ISwitchTechnique
{
  int activeHandAreaIndex { get; }
  int GetFocusedHandAreaIndex();
  void UpdateActiveHandAreaIndex(int i);
}