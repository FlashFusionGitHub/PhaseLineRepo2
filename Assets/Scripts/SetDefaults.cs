using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefaults : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Player 1 settings
        if(!PlayerPrefs.HasKey("MarkerSpeedPlayer0"))
            PlayerPrefs.SetFloat("MarkerSpeedPlayer0", 0.5f);
        if (!PlayerPrefs.HasKey("CameraSpeedPlayer0"))
            PlayerPrefs.SetFloat("CameraSpeedPlayer0", 0.5f);

        //Player 2 settings
        if (!PlayerPrefs.HasKey("MarkerSpeedPlayer1"))
            PlayerPrefs.SetFloat("MarkerSpeedPlayer1", 0.5f);
        if (!PlayerPrefs.HasKey("CameraSpeedPlayer1"))
            PlayerPrefs.SetFloat("CameraSpeedPlayer1", 0.5f);

        //Universal Settings
        if (!PlayerPrefs.HasKey("CursorSpeed"))
            PlayerPrefs.SetFloat("CursorSpeed", 0.5f);
        if (!PlayerPrefs.HasKey("SoundEffects"))
            PlayerPrefs.SetFloat("SoundEffects", 0.5f);
    }
	
}
