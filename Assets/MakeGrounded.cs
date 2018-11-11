using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MakeGrounded : MonoBehaviour {

    NavMeshAgent nma;

    public LayerMask lm;
	// Use this for initialization
	void Start () {
        DoYourThing();
    }

    void DoYourThing()
    {
        nma = GetComponent<NavMeshAgent>();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10000f, lm))
        {
            transform.position = hit.point;
            NavMeshHit nmHit;

            if (NavMesh.SamplePosition(transform.position, out nmHit, 1000f, nma.areaMask))
            {
                transform.position = nmHit.position;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        DestroyImmediate (nma);
        DestroyImmediate(this);
    }
    private void OnDrawGizmos()
    {
        DoYourThing();
    }
}
