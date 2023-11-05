using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitchhikeManager : SingletonMonoBehaviour<HitchhikeManager>
{
    public NetworkHandAreaManager handAreaManager;
    [SerializeField, InterfaceType(typeof(ISwitchTechnique))] Object _switchTechnique;
    private ISwitchTechnique switchTechnique;

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
    }

    void Update()
    {
        var newActiveHandAreaIndex = switchTechnique.GetFocusedHandAreaIndex();
        if (handAreaManager.activeHandAreaIndex != newActiveHandAreaIndex)
        {
            handAreaManager.activeHandAreaIndex = newActiveHandAreaIndex;
            switchTechnique.UpdateActiveHandAreaIndex(newActiveHandAreaIndex);
        }
    }


}
