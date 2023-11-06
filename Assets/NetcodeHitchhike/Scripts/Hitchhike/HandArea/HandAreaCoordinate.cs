using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class HandAreaCoordinate : NetworkBehaviour
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    HandsWrap handsWrap;

    // each coordinate is owned by each player
    public bool isLocal
    {
        get
        {
            if (!IsSpawned) return false;
            return IsOwner;
        }
    }
    private NetworkVariable<bool> n_isOriginal = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public bool isOriginal
    {
        get { return n_isOriginal.Value; }
        set
        {
            if (!IsOwner) return;
            n_isOriginal.Value = value;
        }
    }
    private NetworkVariable<bool> n_isEnabled = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public bool isEnabled
    {
        get { return n_isEnabled.Value; }
        set
        {
            if (!IsOwner) return;
            n_isEnabled.Value = value;
        }
    }

    private void Awake()
    {
        handsWrap = Instantiate(HitchhikeManager.Instance.handsWrapPrefab, HitchhikeManager.Instance.handsWrapPrefab.transform.parent);
        handsWrap.gameObject.SetActive(true);
        handsWrap.coordinate = this;
        if (isOriginal) handsWrap.originalCoordinate = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        n_isEnabled.OnValueChanged += (previousValue, newValue) =>
        {
            meshRenderer.material = newValue ? enabledMaterial : disabledMaterial;
        };
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
