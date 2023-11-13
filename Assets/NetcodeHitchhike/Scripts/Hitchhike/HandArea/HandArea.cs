using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Animations;

public class HandArea : NetworkBehaviour
{
    [SerializeField] NetworkObject handAreaCoordinatePrefab;
    public override void OnNetworkSpawn()
    {
        LocalHitchhikeManager.Instance.handAreaManager.RegisterHandArea(this);
        if (!IsServer) return;
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds) { SpawnCoordinateForClient(id); }
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnCoordinateForClient;
    }

    private void SpawnCoordinateForClient(ulong clientId)
    {
        if (!IsServer) return;
        if (handAreaCoordinatePrefab == null) return;
        NetworkObject n_coordinate = Instantiate(handAreaCoordinatePrefab);
        n_coordinate.transform.localPosition = transform.position;
        n_coordinate.transform.localScale = transform.localScale;
        n_coordinate.transform.rotation = transform.rotation;
        n_coordinate.SpawnWithOwnership(clientId);
        n_coordinate.TrySetParent(transform);
    }

    public HandAreaCoordinate GetCoordinateForClient(ulong clientId)
    {
        return transform.GetComponentsInChildren<HandAreaCoordinate>().First(c => c.GetComponent<NetworkObject>().OwnerClientId == clientId);
    }

    public void RequestDespawn()
    {
        RequestDespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDespawnServerRpc()
    {
        foreach (var coord in GetComponentsInChildren<HandAreaCoordinate>())
        {
            coord.NetworkObject.Despawn();
        }
        NetworkObject.Despawn();
    }
}
