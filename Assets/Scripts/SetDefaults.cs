using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefaults : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Player 1 settings
        if(!PlayerPrefs.HasKey("MarkerSpeedPlayer1"))
            PlayerPrefs.SetFloat("MarkerSpeedPlayer1", 0.5f);
        if (!PlayerPrefs.HasKey("CameraSpeedPlayer1"))
            PlayerPrefs.SetFloat("CameraSpeedPlayer1", 0.5f);

        //Player 2 settings
        if (!PlayerPrefs.HasKey("MarkerSpeedPlayer2"))
            PlayerPrefs.SetFloat("MarkerSpeedPlayer2", 0.5f);
        if (!PlayerPrefs.HasKey("CameraSpeedPlayer2"))
            PlayerPrefs.SetFloat("CameraSpeedPlayer2", 0.5f);

        //Universal Settings
        if (!PlayerPrefs.HasKey("CursorSpeed"))
            PlayerPrefs.SetFloat("CursorSpeed", 0.5f);
        if (!PlayerPrefs.HasKey("SoundEffects"))
            PlayerPrefs.SetFloat("SoundEffects", 0.5f);
    }
	
}
