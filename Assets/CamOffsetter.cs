using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamOffsetter : MonoBehaviour {

    public Transform nestedCam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000f))
        {

        }

        float x = transform.position.y;
        float offset;
        float angle = nestedCam.localEulerAngles.x;
        offset = (x * Mathf.Sin(angle)) / Mathf.Sin(90 - angle);
        nestedCam.localPosition = new Vector3(0, 0, offset);
    }
}
