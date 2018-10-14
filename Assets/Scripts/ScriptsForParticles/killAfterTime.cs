using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killAfterTime : MonoBehaviour {

    [Tooltip("this value will be overwritten by particle duration if on is present")]
    [SerializeField] private float m_timer;
	// Use this for initialization
	void Start () {
        ParticleSystem PS = GetComponent<ParticleSystem>();
        if (PS)
            Destroy(gameObject, PS.main.duration);
        else
            Destroy(gameObject, m_timer);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
