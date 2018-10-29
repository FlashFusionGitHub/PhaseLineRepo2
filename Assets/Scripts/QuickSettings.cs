using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSettings : MonoBehaviour {

    public Slider markerSpeedSlider;
    public Slider cameraSpeedSlider;

    public int playerIndex;

    // Use this for initialization
    void Start () {
        markerSpeedSlider.value = PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex);
        cameraSpeedSlider.value = PlayerPrefs.GetFloat("CameraSpeedPlayer" + playerIndex);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
        PlayerPrefs.SetFloat("MarkerSpeedPlayer" + playerIndex, markerSpeedSlider.value);
        PlayerPrefs.SetFloat("CameraSpeedPlayer" + playerIndex, cameraSpeedSlider.value);
    }
}
