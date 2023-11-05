using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Linq;
using System.Collections.Generic;

public class HandArea : NetworkBehaviour
{
    [SerializeField] NetworkObject handAreaCoordinatePrefab;

    private void Awake()
    {
    }

    public override void OnNetworkSpawn()
    {
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds) { SpawnCoordinateForClient(id); }
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnCoordinateForClient;
    }

    private void SpawnCoordinateForClient(ulong clientId)
    {
        if (!IsServer) return;
        if (handAreaCoordinatePrefab == null) return;
        NetworkObject n_coordinate = Instantiate(handAreaCoordinatePrefab, this.transform);
        n_coordinate.SpawnWithOwnership(clientId);
    }

    // public void Init()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
