using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkHandAreaManager : NetworkBehaviour
{
  public NetworkObject handAreaPrefab;
  public List<HandArea> handAreas { get; private set; }
  int m_activeHandAreaIndex = 0;
  public int activeHandAreaIndex
  {
    get { return m_activeHandAreaIndex; }
    set
    {
      if (handAreas == null) return;
      if (value >= handAreas.Count) return;
      foreach (var item in handAreas.Select((area, index) => new { area, index }))
      {
        // item.area.isEnabled = item.index == value;
      }
      m_activeHandAreaIndex = value;
    }
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    RegisterHandAreas();
    InitHandAreas();
  }

  void RegisterHandAreas()
  {
    handAreas = new List<HandArea>();
    // var original = new List<HandArea>(FindObjectsOfType<HandArea>()).Find(e => e.isOriginal);
    var copied = new List<HandArea>(FindObjectsOfType<HandArea>());
    // handAreas.Add(original);
    handAreas.AddRange(copied);
  }

  void InitHandAreas()
  {
    // handAreas.ForEach(area => area.Init());
    activeHandAreaIndex = 0;
  }

  // if clientId != MaxValue, it creates an original hand area for the client
  public void CreateHandArea(Vector3 position, Quaternion rotation, ulong clientId = ulong.MaxValue)
  {
    CreateHandAreaServerRpc(position, rotation, clientId);
  }

  [ServerRpc]
  private void CreateHandAreaServerRpc(Vector3 position, Quaternion rotation, ulong clientId = ulong.MaxValue)
  {
    var id = CreateHandAreaOnServer(position, rotation);
    if (clientId != ulong.MaxValue && id != ulong.MaxValue) NotifyOriginalIdClientRpc(id, new ClientRpcParams
    {
      Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
    });
  }

  private ulong CreateHandAreaOnServer(Vector3 position, Quaternion rotation)
  {
    if (!IsServer) return ulong.MaxValue;
    if (handAreaPrefab == null) return ulong.MaxValue;
    NetworkObject n_area = Instantiate(handAreaPrefab, position, rotation);
    n_area.Spawn();
    return n_area.NetworkObjectId;
  }

  [ClientRpc]
  private void NotifyOriginalIdClientRpc(ulong id, ClientRpcParams clientRpcParams = default)
  {
    NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().SetOriginalHandArea(id);
  }
}
