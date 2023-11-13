using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public class NetworkGrabbable : NetworkBehaviour
{
    NetworkVariable<bool> isSelected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public void OnSelect()
    {
        if (isSelected.Value) return;
        if (IsOwner) isSelected.Value = true;
        if (!IsOwner) ChangeOwnershipServerRpc();
    }
    public void OnUnselect()
    {
        if (!IsOwner) return;
        isSelected.Value = false;
    }

    private void Update()
    {
        Debug.Log(gameObject.name + ": " + IsOwner);
    }

    [ServerRpc(RequireOwnership = false)]
    void ChangeOwnershipServerRpc(ServerRpcParams serverRpcParams = default)
    {
        NetworkObject.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        NetworkObject.GetComponentsInChildren<NetworkObject>().ToList().ForEach(no =>
        {
            no.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
        });
        isSelected.Value = true;
    }
}
