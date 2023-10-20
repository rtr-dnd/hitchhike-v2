using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_playerPrefab;

    public override void OnNetworkSpawn()
    {
        // base.OnNetworkSpawn();
        NetworkObject player = Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        player.Spawn();
    }

}
