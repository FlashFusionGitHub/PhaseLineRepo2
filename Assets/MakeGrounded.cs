using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGrounded : MonoBehaviour {


    public LayerMask lm;
	// Use this for initialization
	void Start () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10000f, lm))
        {
            transform.position = hit.point;
        }
        else
        {
            Destroy(gameObject);
        }
        Destroy(this);
	}
}
