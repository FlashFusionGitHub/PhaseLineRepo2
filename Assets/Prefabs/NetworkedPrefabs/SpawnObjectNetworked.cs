using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnObjectNetworked : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //[Command]
    //public void CmdSpawn(GameObject spawnThis)
    //{
    //   var go = (GameObject)Instantiate(spawnThis, transform.position, spawnThis.transform.rotation);
    //   NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    //}

    public void Spawn(GameObject spawnThis)
    {
        Instantiate(spawnThis, transform.position, spawnThis.transform.rotation);
    }
}
