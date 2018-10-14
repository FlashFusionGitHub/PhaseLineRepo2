using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathingDrawer : MonoBehaviour {

    LineRenderer lr;
    NavMeshAgent nma;

    float timeUntilNextCheck = 0.1f;
    float m_timer;
	// Update is called once per frame
	void Update () {

        if (!lr || !nma)
        {
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                m_timer = timeUntilNextCheck;
                LookForMyStuff();
                return;
            }
        }
        else if (lr && nma)
        {
            lr.positionCount = nma.path.corners.Length;
            for (int i = 0; i < nma.path.corners.Length; i++)
            {
                lr.SetPosition(i, nma.path.corners[i]);
            }
        }
        
	}

    void LookForMyStuff()
    {
        if (!lr)
        {
            lr = GetComponent<LineRenderer>();
        }
        if (!nma)
        {
            nma = GetComponent<NavMeshAgent>();
        }
    }
}
