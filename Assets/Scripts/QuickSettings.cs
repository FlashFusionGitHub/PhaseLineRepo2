using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSettings : MonoBehaviour {

    public Slider markerSpeedSlider; /*Speed of the navigation marker*/
    public Slider cameraSpeedSlider; /*Camera speed*/

    public int playerIndex;

    // Use this for initialization
    void Start () {
        /*Of start set the defaults of camera and marker speed*/
        markerSpeedSlider.value = PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex);
        cameraSpeedSlider.value = PlayerPrefs.GetFloat("CameraSpeedPlayer" + playerIndex);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save()
    {
        /*Save the playerPref to the corresponding string*/
        PlayerPrefs.SetFloat("MarkerSpeedPlayer" + playerIndex, markerSpeedSlider.value);
        PlayerPrefs.SetFloat("CameraSpeedPlayer" + playerIndex, cameraSpeedSlider.value);
    }
}
