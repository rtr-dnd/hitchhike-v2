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
    public GameObject ovrHands;
    public Material localEnabledMaterial;
    public Material localDisabledMaterial;
    public Material remoteEnabledMaterial;
    public Material remoteDisabledMaterial;
    [HideInInspector] public OVRCameraRig cameraRig;
    [HideInInspector] public Vector3 initialCameraRigPosition;

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
        cameraRig = ovrHands.GetComponentInParent<OVRCameraRig>();
        initialCameraRigPosition = cameraRig.transform.position;
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
