using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkHandAreaManager : NetworkBehaviour
{
  public NetworkObject handAreaPrefab;

  public void CreateHandArea(Vector3 position, Quaternion rotation)
  {
    if (!IsServer) return;
    if (handAreaPrefab == null) return;
    NetworkObject n_area = Instantiate(handAreaPrefab, position, rotation);
    n_area.Spawn();
  }
}
