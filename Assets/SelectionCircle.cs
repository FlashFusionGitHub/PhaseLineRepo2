using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour {

	public TroopActor ta;

	bool taSet;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ((!ta || ta.rankState == RankState.dead) && taSet) {
			Destroy (gameObject);
		}

		if (ta && !taSet) 
		{
			taSet = true;
		}
	}
}
