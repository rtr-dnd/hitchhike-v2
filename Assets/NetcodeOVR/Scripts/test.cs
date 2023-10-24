using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
