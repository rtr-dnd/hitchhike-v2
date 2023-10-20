using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
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

        var position = new Vector3(0, 1, -3);
        position.x = -3 + 2 * (NetworkManager.Singleton.ConnectedClients.Count % 3);
        response.Position = position;
        response.Rotation = Quaternion.identity;

        response.Pending = false;
    }
}
