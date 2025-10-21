using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class TaskImageManager : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer imageForClient0;
    [SerializeField] private SpriteRenderer imageForClient1;
    [SerializeField] private List<Sprite> taskImages;
    //[SerializeField] private int imageIndex = 0;
    [SerializeField] NetworkVariable<int> imageIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        imageIndex.OnValueChanged += OnImageIndexChanged;
        if (IsServer)
        {
            imageIndex.Value = 0;
        }

        OnImageIndexChanged(0, imageIndex.Value);
    }
    
    private void OnImageIndexChanged(int previousValue, int newValue)
    {
        if (newValue < 0 || newValue >= taskImages.Count * 2 + 1)
        {
            Debug.LogError("Invalid image index: " + newValue);
            return;
        }
        imageForClient0.sprite = taskImages[newValue * 2];
        imageForClient1.sprite = taskImages[newValue * 2 + 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (imageIndex.Value == 5)
            {
                imageIndex.Value = 0;
                return;
            }
            imageIndex.Value++;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            imageIndex.Value = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            imageIndex.Value = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            imageIndex.Value = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            imageIndex.Value = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            imageIndex.Value = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            imageIndex.Value = 5;
        }
    }
}
