using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : NetworkBehaviour {

    [SerializeField] private uint roomSize = 2; /*Amount of players per game*/

    private string roomName;

    private NetworkManager networkManager;

    // Use this for initialization
    void Start()
    {
        networkManager = NetworkManager.singleton;

        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void CreateRoom()
    {
        if(roomName != null && roomName != "")
        {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");

            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
