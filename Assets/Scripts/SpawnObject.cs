using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

    public void Spawn(GameObject spawnThis)
    {
        Instantiate(spawnThis, transform.position, spawnThis.transform.rotation);
    }
}
