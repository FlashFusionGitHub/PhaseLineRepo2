using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weed_Spawner : MonoBehaviour {
	public GameObject weed;
	//Tmer
	public float timeUntilNewtumbleWeed;
	public float tumbleWeedTimeRandomiser;
	public float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		timer -= Time.deltaTime;
		if (timer <= 0) {
			Instantiate (weed,this.gameObject.transform.position, this.gameObject.transform.rotation);
			timer = Random.Range (timeUntilNewtumbleWeed - timeUntilNewtumbleWeed * tumbleWeedTimeRandomiser / 100, timeUntilNewtumbleWeed + timeUntilNewtumbleWeed * tumbleWeedTimeRandomiser / 100);
		}
	}
}
