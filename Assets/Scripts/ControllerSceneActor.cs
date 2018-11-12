using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class ControllerSceneActor : MonoBehaviour {

    public SceneLoader sceneLoader;

    public InputDevice[] m_controllers = new InputDevice[2];

    bool[] playersReady = new bool[2];

    public Text readyTxt;

    public GameObject cactus;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            m_controllers[0] = InputManager.Devices[0];
            m_controllers[1] = InputManager.Devices[1];

            if (m_controllers[0].AnyButton.WasPressed)
            {
                playersReady[0] = true;

                if (readyTxt == null)
                    readyTxt.text = "Player 1 ready";
            }

            if (m_controllers[1].AnyButton.WasPressed)
            {
                playersReady[1] = true;

                if (readyTxt == null)
                    readyTxt.text = "Player 2 ready";
            }

            if (playersReady[0] && playersReady[1])
            {
                sceneLoader.LoadScene(3);
                cactus.SetActive(true);
            }
        }
        catch (System.Exception)
        {
            if (Input.anyKeyDown)
            {
                sceneLoader.LoadScene(3);
                DontDestroyOnLoad(cactus);
                cactus.SetActive(true);
            }
        }
	}
}
