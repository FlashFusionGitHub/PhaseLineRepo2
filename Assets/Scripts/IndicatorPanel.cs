using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPanel : MonoBehaviour {

    public NavigationArrowActor m_navMarker;

    public GameObject m_arrow;

    public Camera m_camera;

    //Shitty compass

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 screenPos = m_camera.WorldToScreenPoint(m_navMarker.m_currentMarker.transform.position);

        m_arrow.gameObject.SetActive(true);

        m_arrow.transform.LookAt(screenPos, -Vector3.forward);
        Vector3 rot = m_arrow.transform.eulerAngles;
        rot.x = rot.y = 0;
        m_arrow.transform.eulerAngles = rot;
    }
}
