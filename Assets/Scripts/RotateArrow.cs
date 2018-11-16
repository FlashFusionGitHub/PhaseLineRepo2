using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/*Rotate object class*/
public class RotateArrow : MonoBehaviour {

    public Controller m_controller; /*Reference to the Controller Class*/

    public int playerIndex; /*players index*/

	public float rotateSpeed = 100f;
	
	// Update is called once per frame
	void Update () {
        if (m_controller == null)
        {
            foreach (Controller c in FindObjectsOfType<Controller>())
            {
                if (playerIndex == 0 && c.m_playerIndex == 0)
                {
                    m_controller = c;
                }

                if (playerIndex == 1 && c.m_playerIndex == 1)
                {
                    m_controller = c;
                }
            }
        }

        if (m_controller.LeftBumperIsHeld())
			transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        if (m_controller.RightBumperIsHeld())
			transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
