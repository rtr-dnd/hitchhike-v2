using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
namespace NetcodeHitchhike.BlockTask
{
    public class BlockPlacer : NetworkBehaviour
    {
        [SerializeField] private Vector3 defaultPosition;
        [SerializeField] private Quaternion defaultRotation;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnNetworkSpawn()
        {
            defaultPosition = this.transform.position;
            defaultRotation = this.transform.rotation;
        }

        // Update is called once per framer
        public void Reset()
        {
            if (!IsOwner) return;
            this.transform.position = defaultPosition;
            this.transform.rotation = defaultRotation;
        }
    }
}