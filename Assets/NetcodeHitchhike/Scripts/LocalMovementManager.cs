using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalMovementManager : MonoBehaviour
{
    public Transform headAnchor;
    PlayerMovementPool playerMovementPool;

    void Update()
    {
        if (playerMovementPool == null)
        {
            playerMovementPool = FindObjectsOfType<PlayerMovementPool>().First(pool => pool.IsOwner);
            if (playerMovementPool == null) return;
        }
        playerMovementPool.hmdPosePool.Value = new Pose(headAnchor.position, headAnchor.rotation);
    }
}
