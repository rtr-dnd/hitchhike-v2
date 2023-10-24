using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class NetcodeOVRPlayer : NetworkBehaviour
{
    DisablePlayer disablePlayer;
    // Start is called before the first frame update
    void Start()
    {
        // if (!IsOwner)
        // {
        //     disablePlayer.DisableScripts();
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
