using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour {

    public bool taken;

    public CheckForGrounded moveTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!moveTarget && taken)
        {
            clearPoI();
        }

	}


    public void assignMoveTarget(CheckForGrounded mt)
    {
        moveTarget = mt;
        taken = true;
    }

    public void clearPoI()
    {
        moveTarget = null;
        taken = false;
    }
}
