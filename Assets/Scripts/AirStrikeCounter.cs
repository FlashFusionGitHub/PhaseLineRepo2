using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirStrikeCounter : MonoBehaviour {

	public NavigationArrowActor navArrowActor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Text> ().text = navArrowActor.airstrikes.Count.ToString();
	}
}
