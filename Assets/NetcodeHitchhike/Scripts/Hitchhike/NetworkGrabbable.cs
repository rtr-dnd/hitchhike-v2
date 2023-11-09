using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkGrabbable : NetworkBehaviour
{
    public void OnSelect()
    {
        if (!IsOwner) ChangeOwnershipServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ChangeOwnershipServerRpc(ServerRpcParams serverRpcParams = default)
    {
        NetworkObject.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
    }
}
