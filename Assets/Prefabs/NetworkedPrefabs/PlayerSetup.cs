using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField] Behaviour[] componentsToDisable;

    Camera sceneCamera;

    [SerializeField] GameObject[] prefabs;

    [SerializeField] Vector3[] teamTwoSpawnPoints;

    // Use this for initialization
    void Start () {

        if(isClient && !isServer)
        {
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }

            CmdSpawnTeamTwo();

            FindObjectOfType<ObjectPoolNetworked>().FindAllTroopTargets();
            FindObjectOfType<ObjectPoolNetworked>().SplitTroops();
            FindObjectOfType<ObjectPoolNetworked>().FindAllGenerals();
        }
        else
        {
            sceneCamera = Camera.main;

            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            FindObjectOfType<ObjectPoolNetworked>().FindAllTroopTargets();
            FindObjectOfType<ObjectPoolNetworked>().SplitTroops();
            FindObjectOfType<ObjectPoolNetworked>().FindAllGenerals();
        }
    }

    private void OnDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

    [Command]
    void CmdSpawnTeamTwo()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].gameObject.GetComponent<TroopActorNetworked>().team == Team.TEAM2)
            {
                var go = (GameObject)Instantiate(prefabs[i], teamTwoSpawnPoints[i], Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
            }
        }
    }
}
