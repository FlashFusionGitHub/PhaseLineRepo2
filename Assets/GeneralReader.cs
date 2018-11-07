using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralReader : MonoBehaviour {
    public Team team;

    Text txt;

    public ObjectPool op;
	// Use this for initialization
	void Start () {
        op = FindObjectOfType<ObjectPool>();
	}
	
	// Update is called once per frame
	void Update () {

        //op.team1Generals.Count;
	}
}
