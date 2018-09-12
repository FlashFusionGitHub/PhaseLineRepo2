using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class CameraController : MonoBehaviour {

    public float m_MinZoom = 10.0f, m_MaxZoom = 50.0f;
    public float m_MinPanX = -50.0f, m_MaxPanX = 50.0f;
    public float m_MinPanZ = -50.0f, m_MaxPanZ = 50.0f;

    public float slerpSpeed = 50;

    Vector3 position;

    public bool changePosition;

    public InputDevice m_controller;

    public int m_playerIndex;

	public float cameraSpeed;

    // Use this for initialization
    protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {

        m_controller = InputManager.Devices[m_playerIndex];

        if (!changePosition)
        {
            if (m_controller.RightTrigger.IsPressed)
            {
				transform.position += new Vector3(0, -m_controller.RightStickY * cameraSpeed, 0);

                float zoom = Mathf.Clamp(transform.position.y, m_MinZoom, m_MaxZoom);

                transform.position = new Vector3(transform.position.x, zoom, transform.position.z);
            }
            else
            {
				this.transform.position += new Vector3(m_controller.RightStickX  * cameraSpeed, 0, m_controller.RightStickY  * cameraSpeed);

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

    public void MoveCameraTo(float x, float z)
    {
        changePosition = true;
        position = new Vector3(x, transform.position.y, z);
    }
}
