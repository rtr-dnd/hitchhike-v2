using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisplaceCameraRigOnSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = NetworkManager.LocalClient.PlayerObject.transform.position;
    }
}
