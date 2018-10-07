using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMarkerNetworked : MonoBehaviour {

    public GameObject m_navMarker;
    public GameObject m_currentMarker;

    public float m_markerSpeed = 2;
    public float m_minXPos = -100, maxXPos = 100;
    public float m_minZPos = -100, maxZPos = 100;

    InputDevice m_controller;

    // Use this for initialization
    void Start () {
        m_currentMarker = Instantiate(m_navMarker, new Vector3(0, 4, 0), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        m_controller = InputManager.Devices[0];

        float markerXPos = Mathf.Clamp(m_currentMarker.transform.position.x, m_minXPos, maxXPos);
        float markerZPos = Mathf.Clamp(m_currentMarker.transform.position.z, m_minZPos, maxZPos);

        m_currentMarker.transform.position = new Vector3(markerXPos, m_currentMarker.transform.position.y, markerZPos);

        m_currentMarker.transform.position += new Vector3(m_controller.LeftStickX, 0, m_controller.LeftStickY) * m_markerSpeed;
    }
}
