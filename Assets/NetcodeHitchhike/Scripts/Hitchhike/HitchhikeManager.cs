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
    public DrivenHandVisual drivenHandPrefabLeft;
    public DrivenHandVisual drivenHandPrefabRight;
    public GameObject ovrHands;
    public Material localEnabledMaterial;
    public Material localDisabledMaterial;
    public Material remoteEnabledMaterial;
    public Material remoteDisabledMaterial;
    public bool scaleHandModel;
    public bool billboardToHead = true;
    public Transform headAnchor;
    [HideInInspector] public OVRCameraRig cameraRig;
    [HideInInspector] public Vector3 initialCameraRigPosition;

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
        cameraRig = ovrHands.GetComponentInParent<OVRCameraRig>();
        initialCameraRigPosition = cameraRig.transform.position;
    }
}
