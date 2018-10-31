using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class CameraController : MonoBehaviour {

    public float m_MinZoomY = 10.0f, m_MaxZoomY = 50.0f;
    public float m_MinZoomZ = 10.0f, m_MaxZoomZ = 50.0f;
    public float m_MinPanX = -50.0f, m_MaxPanX = 50.0f;
    public float m_MinPanZ = -50.0f, m_MaxPanZ = 50.0f;

    public float slerpSpeed = 50;

    Vector3 position;

    public bool changePosition;

    public InputDevice m_controller;

    public int m_playerIndex;

	public float cameraSpeed;

    public Camera camera;

    public float offset;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        try
        {
            m_controller = InputManager.Devices[m_playerIndex];
        }
        catch (System.Exception)
        {
            return;
        }

        if (!changePosition)
        {
            float x = transform.position.x;
            float z = transform.position.z;

            if (m_controller.RightTrigger.IsPressed)
            {
				transform.position += new Vector3(0, -m_controller.RightStickY * Time.deltaTime * (PlayerPrefs.GetFloat("CameraSpeedPlayer" + m_playerIndex) * 1000), m_controller.RightStickY * Time.deltaTime * (PlayerPrefs.GetFloat("CameraSpeedPlayer" + m_playerIndex) * 1000));

                float zoomY = Mathf.Clamp(transform.position.y, m_MinZoomY, m_MaxZoomY);

                transform.position = new Vector3(x, zoomY, z);
            }
            else
            {
				this.transform.position += new Vector3(m_controller.RightStickX * Time.deltaTime * (PlayerPrefs.GetFloat("CameraSpeedPlayer" + m_playerIndex) * 1000), 0, m_controller.RightStickY * Time.deltaTime * (PlayerPrefs.GetFloat("CameraSpeedPlayer" + m_playerIndex) * 1000));

                float panX = Mathf.Clamp(transform.position.x, m_MinPanX, m_MaxPanX);
                float panZ = Mathf.Clamp(transform.position.z, m_MinPanZ, m_MaxPanZ);

                transform.position = new Vector3(panX, transform.position.y, panZ);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, position, slerpSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, position) <= 1)
            {
                changePosition = false;
            }
        }
    }


    public void MoveCameraTo(Vector3 targetPos)
    {
        changePosition = true;

        Vector3 oldPos = transform.position;
        Vector3 fwd = camera.transform.forward;

        position = new Vector3(
            targetPos.x + fwd.x / fwd.y * (oldPos.y - targetPos.y),
            oldPos.y,
            targetPos.z + fwd.z / fwd.y * (oldPos.y - targetPos.y));
    }
}
