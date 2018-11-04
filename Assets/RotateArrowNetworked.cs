using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class RotateArrowNetworked : MonoBehaviour {

    InputDevice m_controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            m_controller = InputManager.Devices[0];
        }
        catch (System.Exception)
        {
            return;
        }

        if (m_controller.LeftBumper.IsPressed)
            transform.Rotate(Vector3.down * 50 * Time.deltaTime);
        if (m_controller.RightBumper.IsPressed)
            transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
