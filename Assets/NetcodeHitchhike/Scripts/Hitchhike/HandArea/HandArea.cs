using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Animations;

public class HandArea : NetworkBehaviour
{
    [SerializeField] NetworkObject handAreaCoordinatePrefab;

    private void Awake()
    {
    }

    public override void OnNetworkSpawn()
    {
        HitchhikeManager.Instance.handAreaManager.RegisterHandArea(this);
        if (!IsServer) return;
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds) { SpawnCoordinateForClient(id); }
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnCoordinateForClient;
    }

    private void SpawnCoordinateForClient(ulong clientId)
    {
        if (!IsServer) return;
        if (handAreaCoordinatePrefab == null) return;
        NetworkObject n_coordinate = Instantiate(handAreaCoordinatePrefab);
        n_coordinate.SpawnWithOwnership(clientId);
        n_coordinate.TrySetParent(transform, false);
    }

    public HandAreaCoordinate GetCoordinateForClient(ulong clientId)
    {
        return transform.GetComponentsInChildren<HandAreaCoordinate>().First(c => c.GetComponent<NetworkObject>().OwnerClientId == clientId);
    }

    // public void Init()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
