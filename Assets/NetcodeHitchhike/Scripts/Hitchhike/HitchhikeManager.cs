using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class HitchhikeManager : SingletonMonoBehaviour<HitchhikeManager>
{
    public NetworkHandAreaManager handAreaManager;
    [SerializeField, InterfaceType(typeof(ISwitchTechnique))] Object _switchTechnique;
    public ISwitchTechnique switchTechnique;
    public HandsWrap handsWrapPrefab;

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
    }

    // void Update()
    // {
    //     localNetworkPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
    //     var newActiveHandAreaIndex = switchTechnique.GetFocusedHandAreaIndex();
    //     if (localNetworkPlayer.activeHandAreaIndex.Value != newActiveHandAreaIndex)
    //     {
    //         localNetworkPlayer.activeHandAreaIndex.Value = newActiveHandAreaIndex;
    //         switchTechnique.UpdateActiveHandAreaIndex(newActiveHandAreaIndex);
    //     }
    // }
}
