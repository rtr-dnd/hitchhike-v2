using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public class NetworkGrabbable : NetworkBehaviour
{
    bool isSelected;
    public void OnSelect()
    {
        if (isSelected) return;
        if (!IsOwner) ChangeOwnershipServerRpc();
        isSelected = true;
    }
    public void OnUnselect()
    {
        isSelected = false;
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
