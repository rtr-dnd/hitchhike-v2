using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HandAreaCoordinate : NetworkBehaviour
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;
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
        set { n_isOriginal.Value = value; }
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
            n_isEnabled.Value = value;
            meshRenderer.material = value ? enabledMaterial : disabledMaterial;
        }
    }
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
