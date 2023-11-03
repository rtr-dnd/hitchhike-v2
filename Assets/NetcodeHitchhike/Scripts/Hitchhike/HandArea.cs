using UnityEngine;

public class HandArea : MonoBehaviour
{
    public bool isOriginal = false;
    private bool m_isEnabled = false;
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;
    public bool isEnabled
    {
        get { return m_isEnabled; }
        set
        {
            m_isEnabled = value;
            meshRenderer.material = value ? enabledMaterial : disabledMaterial;
        }
    }
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Init()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
