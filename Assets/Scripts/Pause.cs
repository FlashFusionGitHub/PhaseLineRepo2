using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.Events;

public class Pause : MonoBehaviour {

    Controller m_controller;

    public GameObject m_pausePanel;

    public GameObject cursor;

    public int playerIndex;

	// Use this for initialization
	void Start () {
        cursor.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if(m_controller == null)
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

        if (Time.timeScale == 0)
            return;
        else
        {
            if (m_controller.MenuWasPress())
            {
                PauseGame();
            }
        }
	}

    void PauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().TweenToOutPos();

        Time.timeScale = 0;

        cursor.SetActive(true);
    }

    public void UnpauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().TweenToInPos();

        Time.timeScale = 1;

        cursor.SetActive(false);
    }
}
