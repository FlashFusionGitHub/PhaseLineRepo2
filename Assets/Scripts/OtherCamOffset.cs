using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCamOffset : MonoBehaviour {

    public Transform camContainer;
    public Transform cam;
    public LayerMask lm;
    public float rotSpeed = 10f;

    Vector3 lookAtThis;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(camContainer.position.x, 10000f, camContainer.position.z), Vector3.down, out hit, 50000f, lm))
        {
            lookAtThis = hit.point;
        
        }
            
        if (lookAtThis != Vector3.zero)
            cam.LookAt(lookAtThis);
        cam.localEulerAngles = new Vector3(cam.localEulerAngles.x, 0, 0);
    }
}
