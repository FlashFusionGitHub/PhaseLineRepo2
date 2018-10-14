using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.Events;

public class Pause : MonoBehaviour {

    InputDevice m_controller;

    public GameObject m_pausePanel;

    public Text m_title;

    int playerIndex = 99;

    UnityEvent pauseEvent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_controller = InputManager.Devices[0];

        if(m_controller.MenuWasPressed)
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
	}

    void PauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().ToggleInOut();

        Time.timeScale = 0;

        m_title.text = "Player " + playerIndex + " Paused";

    }

    public void UnpauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().ToggleInOut();

        Time.timeScale = 1;
    }
}
