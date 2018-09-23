using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleweedMovement : MonoBehaviour {
	//public Quaternion Target;
	public Vector3 Moveto;
	public Vector3 coordinates;
	public float smoothSpeed;
	//public float[] rotateSpeed;
	public float moveSpeed;
	public Rigidbody rb;
	public float currentVelocity;

	//forceTimer
	public float m_forceTimer;
	public float m_timeUntilNewForce;
	public Vector3 minForceToAdd;
	public Vector3 maxForceToAdd;

	//Velocity Reader
	public Vector3 velocityReader;

	//Timer
	public float Timer;
	// Use this for initialization
	void Start () {
		coordinates.x = Random.Range (100, 150);
		coordinates.y = -50f;
		coordinates.z = Random.Range (100, 150);
		Moveto = coordinates;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate (rotateSpeed [0], rotateSpeed [1], rotateSpeed [2]);
		Timer -= Time.deltaTime; 
		//rb.velocity = Moveto;
		if (m_forceTimer <= 0) {
			rb.AddForce (new Vector3 (Random.Range (minForceToAdd.x, maxForceToAdd.x), Random.Range (minForceToAdd.y, maxForceToAdd.y), Random.Range (minForceToAdd.z, maxForceToAdd.z)) * Time.deltaTime);
			m_forceTimer = m_timeUntilNewForce;
		} else 
		{
			m_forceTimer -= Time.deltaTime;
		}
		velocityReader = rb.velocity;

		if (Timer <= 0) {
			Destroy (this.gameObject);
		}
		if (velocityReader.z == 0f && Timer <= 30f) {
			Destroy (gameObject);
		}
	}
}
