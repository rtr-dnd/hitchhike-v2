using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitchhikeManager : SingletonMonoBehaviour<HitchhikeManager>
{
    public List<HandArea> handAreas { get; private set; }
    [SerializeField, InterfaceType(typeof(ISwitchTechnique))] Object _switchTechnique;
    private ISwitchTechnique switchTechnique;
    int m_activeHandAreaIndex = 0;
    public int activeHandAreaIndex
    {
        get { return m_activeHandAreaIndex; }
        private set
        {
            if (handAreas == null) return;
            if (value >= handAreas.Count) return;
            foreach (var item in handAreas.Select((area, index) => new { area, index }))
            {
                // item.area.isEnabled = item.index == value;
            }
            m_activeHandAreaIndex = value;
            switchTechnique.UpdateActiveHandAreaIndex(value);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
    }

    private void Start()
    {
        RegisterHandAreas();
        InitHandAreas();
    }

    void Update()
    {
        var newActiveHandAreaIndex = switchTechnique.GetFocusedHandAreaIndex();
        if (activeHandAreaIndex != newActiveHandAreaIndex) activeHandAreaIndex = newActiveHandAreaIndex;
    }

    void RegisterHandAreas()
    {
        handAreas = new List<HandArea>();
        // var original = new List<HandArea>(FindObjectsOfType<HandArea>()).Find(e => e.isOriginal);
        // var copied = new List<HandArea>(FindObjectsOfType<HandArea>()).FindAll(e => !e.isOriginal);
        // handAreas.Add(original);
        // handAreas.AddRange(copied);
    }

    void InitHandAreas()
    {
        // handAreas.ForEach(area => area.Init());
        activeHandAreaIndex = 0;
    }
}
