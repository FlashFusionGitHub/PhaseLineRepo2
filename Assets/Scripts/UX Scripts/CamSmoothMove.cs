using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CamSmoothMove : MonoBehaviour {

    [SerializeField] private Transform m_moveTarget;
    [SerializeField] private Transform m_lookTarget;
    [SerializeField] private float m_moveSpeed = 10f;
    [SerializeField] private float m_lookSpeed = 10f;

    Rigidbody m_rigidbody;

	// Use this for initialization
	void Start () {
        m_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        if (m_moveTarget)
        {
            if (Vector3.Distance(transform.position, m_moveTarget.position) >= m_moveSpeed * 0.01f)
            {
                m_rigidbody.MovePosition(transform.position + (m_moveTarget.position - transform.position).normalized * m_moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = m_moveTarget.position;
            }
        }


        if (m_lookTarget)
        m_rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((m_lookTarget.position - transform.position).normalized), m_lookSpeed * Time.deltaTime));
	}

    public void SetMoveTarget(Transform newMoveTarget)
    {
        m_moveTarget = newMoveTarget;
    }

    public void SetLookTarget(Transform newLookTarget)
    {
        m_lookTarget = newLookTarget;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        m_moveSpeed = newMoveSpeed;
    }
}
