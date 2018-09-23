using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Cinematic : MonoBehaviour {
	public Vector3[] location;
	public int locationno;
	public Transform target;
	public Quaternion Target;
	public float smoothSpeed;
	public int tiltNo;
	public float[] tiltAroundZ;
	public float[] tiltAroundX;
	public float[] tiltAroundY;

	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 desiredPosition = location [locationno];
		Vector3 smoothedPosition = Vector3.Lerp (transform.position, desiredPosition, smoothSpeed);
		Target = Quaternion.Euler (tiltAroundX [tiltNo], tiltAroundY [tiltNo], tiltAroundZ [tiltNo]);

		transform.position = smoothedPosition;
		transform.rotation = Quaternion.Slerp (transform.rotation, Target, smoothSpeed);
	
		
	}
}
