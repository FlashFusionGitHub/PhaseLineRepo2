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

    bool waitForUnPause;


    float wait = 2;
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

        if (m_controller.MenuWasPressed)
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

        /*if(waitForUnPause)
        {
            wait -= Time.deltaTime;

            if (wait <= 0)
            {
                m_pausePanel.SetActive(false);
                Time.timeScale = 1;
                wait = 2f;
                waitForUnPause = false;
            }
        }*/
	}

    void PauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().ToggleInOut();

        Time.timeScale = 0;

        m_title.text = "Player " + playerIndex + " Paused";

        m_pausePanel.SetActive(true);

    }

    public void UnpauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().ToggleInOut();

        waitForUnPause = true;
    }
}
