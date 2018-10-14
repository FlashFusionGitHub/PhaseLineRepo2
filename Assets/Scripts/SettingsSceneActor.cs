using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSceneActor : MonoBehaviour {

    public GameObject GameSettingsPanel;
    public GameObject ScreenSettingsPanel;
    public GameObject SoundSettingsPanel;

    public GameObject[] panels;

    public Slider markerSpeed;
    public Slider cameraSpeed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //PlayerPrefs.SetFloat("marker", );
	}

    public void OpenGameSettingsPanel()
    {
        GameSettingsPanel.SetActive(true);
        ScreenSettingsPanel.SetActive(false);
        SoundSettingsPanel.SetActive(false);
    }

    public void OpenScreenSettingsPanel()
    {
        GameSettingsPanel.SetActive(false);
        ScreenSettingsPanel.SetActive(true);
        SoundSettingsPanel.SetActive(false);
    }

    public void OpenSoundSettingsPanel()
    {
        GameSettingsPanel.SetActive(false);
        ScreenSettingsPanel.SetActive(false);
        SoundSettingsPanel.SetActive(true);
    }
}
