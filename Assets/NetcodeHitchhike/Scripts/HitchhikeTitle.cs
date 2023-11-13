using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HitchhikeTitle : MonoBehaviour
{
    public string gameSceneName = "Game";
    public void StartHost()
    {
        // todo: remove player prefab
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Pending = true;
        if (NetworkManager.Singleton.ConnectedClients.Count >= 4)
        {
            response.Approved = false;
            response.Pending = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        var position = new Vector3(0, 0, 0)
        {
            x = -1 + (NetworkManager.Singleton.ConnectedClients.Count % 3)
        };
        response.Position = position;
        response.Rotation = Quaternion.identity;

        response.Pending = false;
    }
}
