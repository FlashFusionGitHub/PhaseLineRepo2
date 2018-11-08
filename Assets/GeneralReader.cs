using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralReader : MonoBehaviour {
    public Team team;

    Text txt;

	int GeneralCount;

    public ObjectPool op;
	// Use this for initialization
	void Start () {
        op = FindObjectOfType<ObjectPool>();
		txt = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
			if (GeneralCount !=  ((team == Team.TEAM1) ? op.team1Generals.Count : op.team2Generals.Count)) {
				GeneralCount = (team == Team.TEAM1) ? op.team1Generals.Count : op.team2Generals.Count;
				txt.text = (GeneralCount.ToString());
			}
	}
}
