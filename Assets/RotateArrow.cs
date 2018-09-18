using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class RotateArrow : MonoBehaviour {

    public InputDevice m_controller;

    public Team team;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (team == Team.TEAM1)
            m_controller = InputManager.Devices[0];
        if (team == Team.TEAM2)
            m_controller = InputManager.Devices[1];

        if (m_controller.LeftBumper.IsPressed)
            transform.Rotate(Vector3.down * 50 * Time.deltaTime);
        if (m_controller.RightBumper.IsPressed)
            transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
