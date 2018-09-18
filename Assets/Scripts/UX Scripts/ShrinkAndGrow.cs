using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAndGrow : MonoBehaviour {

    [Header("Grow Settings")]
    [SerializeField] private bool m_growing;
    [SerializeField] private Vector3 m_maxSize;
    [SerializeField] private float m_growSpeed;

    [Header ("Original Size Settings")]
    [SerializeField] private bool m_reseting;
    [SerializeField] private Vector3 m_normalSize;
    [SerializeField] private float m_resetSpeed;

    [Header ("Shrink Settings")]
    [SerializeField] private bool m_shrinking;
    [SerializeField] private Vector3 m_smallSize;
    [SerializeField] private float m_shrinkSpeed;

    [Header("Buffer")]
    [SerializeField] private float m_shrinkingBuffer = 0.99f;
    [SerializeField] private float m_growingBuffer = 1.01f;
    [SerializeField] private float m_resetingBuffer = 0.99f;

    // Use this for initialization
    void Start () {
        m_normalSize = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_shrinking)
        {
            Shrink();
        }
        else if (m_growing)
        {
            Grow();
        }
        else if (m_reseting)
        {
            GoBackToNormal();
        }
    }

    void GoBackToNormal()
    {
        if (transform.localScale.x >= m_normalSize.x || transform.localScale.y >= m_normalSize.y || transform.localScale.z >= m_normalSize.z)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, m_normalSize - (m_normalSize * m_resetingBuffer), m_resetSpeed * Time.deltaTime);
        }
        else if (transform.localScale.x <= m_normalSize.x || transform.localScale.y <= m_normalSize.y || transform.localScale.z <= m_normalSize.z)
        {
            m_reseting = false;
            transform.localScale = m_normalSize;
        }
    }
    void Grow()
    {
        if (transform.localScale.x <= m_maxSize.x || transform.localScale.y <= m_maxSize.y || transform.localScale.z <= m_maxSize.z)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, m_maxSize + (m_maxSize * m_growingBuffer), m_growSpeed * Time.deltaTime);
        }
        else if (transform.localScale.x >= m_maxSize.x || transform.localScale.y >= m_maxSize.y || transform.localScale.z >= m_maxSize.z)
        {
            m_growing = false;
            m_reseting = true;
        }
    }
    void Shrink()
    {
        if (transform.localScale.x >= m_smallSize.x || transform.localScale.y >= m_smallSize.y || transform.localScale.z >= m_smallSize.z)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, m_smallSize - ( m_smallSize* m_shrinkingBuffer), m_shrinkSpeed * Time.deltaTime);
        }
        else if (transform.localScale.x <= m_smallSize.x && transform.localScale.y <= m_smallSize.y && transform.localScale.z <= m_smallSize.z)
        {
            m_shrinking = false;
            m_growing = true;
        }
    }
    public void SelectObj()
    {
        if (m_growing)
            m_growing = false;
        if (m_reseting)
            m_reseting = false;
        m_shrinking = true;
    }

    public void SetGrowState(bool state)
    {
        m_growing = state;
    }

    public void SetShrinkState(bool state)
    {
        m_shrinking = state;
    }
}
