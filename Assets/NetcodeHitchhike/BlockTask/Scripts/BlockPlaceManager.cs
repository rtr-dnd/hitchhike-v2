using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
namespace NetcodeHitchhike.BlockTask
{
    public class BlockPlaceManager : MonoBehaviour
    {
        [SerializeField] private List<BlockPlacer> blockPlacers;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            blockPlacers = this.GetComponentsInChildren<BlockPlacer>().ToList();
        }

        void Update()
        {
         if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }   
        }

        // Update is called once per frame
        public void Reset()
        {
            foreach (var placer in blockPlacers)
            {
                placer.Reset();
            }
        }
    }
}