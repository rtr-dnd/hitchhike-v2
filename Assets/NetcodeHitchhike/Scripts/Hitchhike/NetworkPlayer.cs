using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner) Invoke(nameof(Displace), 0.5f);
    }
    void Displace()
    {
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = transform.position;
        var handAreaManager = GetComponent<NetworkHandAreaManager>();
        handAreaManager.CreateHandArea(new Vector3(
            transform.position.x,
            0.7f,
            transform.position.z + 0.3f
        ), Quaternion.identity);
    }
}
