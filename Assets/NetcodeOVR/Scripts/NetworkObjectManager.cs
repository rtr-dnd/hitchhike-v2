using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkObjectManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_prefab;

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            // base.OnNetworkSpawn();
            NetworkObject no = Instantiate(m_prefab, new Vector3(0, 0.6f, 0), Quaternion.identity);
            no.Spawn();
        }
    }

}
