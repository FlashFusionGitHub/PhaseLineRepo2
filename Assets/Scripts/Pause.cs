using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

    InputDevice[] m_controller;

    public GameObject m_pausePanel;

    public Text m_title;

    int playerIndex = 99;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //m_controller[0] = InputManager.Devices[0];
        //m_controller[1] = InputManager.Devices[1];

        //if (m_controller[0].MenuWasPressed)
        //    playerIndex = 1;
        //if (m_controller[1].MenuWasPressed)
        //    playerIndex = 2;

        //if(m_controller.MenuWasPressed)
        //{
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
        }
        //}
	}

    void PauseGame()
    {
        Time.timeScale = 0;

        m_pausePanel.SetActive(true);

        //Pause Panel 
        m_title.text = "Player " + playerIndex + " Paused";

    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;

        m_pausePanel.SetActive(false);
    }
}
