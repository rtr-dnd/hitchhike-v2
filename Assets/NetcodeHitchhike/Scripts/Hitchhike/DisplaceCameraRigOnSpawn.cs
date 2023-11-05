using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisplaceCameraRigOnSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        transform.position = NetworkManager.LocalClient.PlayerObject.transform.position;
    }
}
