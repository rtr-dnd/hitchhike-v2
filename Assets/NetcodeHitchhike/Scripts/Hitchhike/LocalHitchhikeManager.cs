using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LocalHitchhikeManager : SingletonMonoBehaviour<LocalHitchhikeManager>
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
    public bool DragAndDrop = true;
    [HideInInspector] public OVRCameraRig cameraRig;
    [HideInInspector] public Vector3 initialCameraRigPosition;
    [HideInInspector] public Transform beforeCoordTransform;

    protected override void Awake()
    {
        base.Awake();
        switchTechnique = _switchTechnique as ISwitchTechnique;
        cameraRig = ovrHands.GetComponentInParent<OVRCameraRig>();
        initialCameraRigPosition = cameraRig.transform.position;
        beforeCoordTransform = new GameObject("before coord transform").transform;
    }

    public Vector3 GetCameraRigDisplace()
    {
        return cameraRig.transform.position - initialCameraRigPosition;
    }
}
