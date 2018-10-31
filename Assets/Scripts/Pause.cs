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

        /*If the timescale is ZERO, it means the game is already paused (This stops another player from overriding the pause screen)*/
        if (Time.timeScale == 0)
            return;
        else
        {
            /*Menu button pressed, call PauseGame()*/
            if (m_controller.MenuWasPress())
            {
                PauseGame();
            }
        }
	}

    void PauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().TweenToOutPos(); /*Tween to out position*.

        Time.timeScale = 0; /*Setting time scale to zero stops the game*/

        cursor.SetActive(true); /*Enable the pause panel cursor, to navigate menus*/
    }

    public void UnpauseGame()
    {
        m_pausePanel.GetComponent<TweenAnimator>().TweenToInPos(); /*Tween back to in position*/

        Time.timeScale = 1; /*Set Time scale back to 1 to unpause*/

        cursor.SetActive(false); /*Disable the pause panel cursor*/
    }
}
