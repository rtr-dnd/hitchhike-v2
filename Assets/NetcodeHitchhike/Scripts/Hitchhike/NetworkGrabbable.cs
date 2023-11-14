using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        NetworkObject.GetComponentsInChildren<NetworkObject>().ToList().ForEach(no =>
        {
            no.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        });
    }
}
