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

    public float floatValue = 1f;

    public LayerMask terrainMask;

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

        var objPos = m_currentMarker.transform.position;

        RaycastHit hit;

        if (Physics.Raycast(new Vector3(objPos.x, 500, objPos.z), Vector3.down, out hit, 800f, terrainMask))
        {
            Debug.DrawLine(new Vector3(objPos.x, 500, objPos.z), hit.point);
            m_currentMarker.transform.Translate(0, (floatValue - hit.distance), 0);
            m_currentMarker.transform.position += new Vector3(m_controller.LeftStickX, 0, m_controller.LeftStickY) * m_markerSpeed * Time.deltaTime;
            m_currentMarker.transform.position = new Vector3(m_currentMarker.transform.position.x, hit.point.y, m_currentMarker.transform.position.z);
        }
        else
        {
            m_currentMarker.transform.position += new Vector3(m_controller.LeftStickX, 0, m_controller.LeftStickY) * m_markerSpeed * Time.deltaTime;
        }
    }
}
