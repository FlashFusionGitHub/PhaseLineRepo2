using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsSelected : MonoBehaviour {

    public FactionSelectionCursor[] fsc = new FactionSelectionCursor[2];

    public SceneLoader sceneLoader;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (fsc[0].playerChosen && fsc[1].playerChosen)
            sceneLoader.LoadScene(2);
	}
}
