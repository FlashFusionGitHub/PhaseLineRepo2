using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicBox : MonoBehaviour {

    AudioSource audioSource;

    public AudioClip[] musicArray;
	// Use this for initialization
	void Start () {
		
	}

    public void loadMusic(int musicIndex)
    {
        if (musicArray[musicIndex])
        {
            audioSource.clip = musicArray[musicIndex];
            audioSource.Play();
        }
    }
}
