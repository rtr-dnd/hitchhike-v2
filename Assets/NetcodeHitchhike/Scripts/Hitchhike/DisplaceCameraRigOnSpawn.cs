using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisplaceCameraRigOnSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        Invoke(nameof(Displace), 0.5f);
    }
    void Displace()
    {
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = transform.position;
    }
}
