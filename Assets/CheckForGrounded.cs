using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CheckForGrounded : MonoBehaviour {

    Transform origin;
    public NavMeshAgent TempNma;

    Vector3 prevPos;

	// Use this for initialization
	void Awake () {
        TempNma = GetComponent<NavMeshAgent>();
        TempNma.enabled = false;
        origin = new GameObject(gameObject.name + "'s Origin").transform;

        origin.parent = transform.parent;
        origin.localPosition = transform.localPosition;
        origin.localRotation = transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (origin.parent != transform.parent)
        {
            origin.parent = transform.parent;
            origin.localPosition = transform.localPosition;
        }
        NavMeshHit nHit;
        if (origin.position != prevPos)
        {
            if (NavMesh.SamplePosition(origin.position, out nHit, 2000f, TempNma.areaMask))
            {
                transform.position = nHit.position;
                prevPos = origin.position;
            }
        }
    }
}
