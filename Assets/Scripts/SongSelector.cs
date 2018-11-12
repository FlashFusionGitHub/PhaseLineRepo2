using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelector : MonoBehaviour {

    public AudioController ac;
    public int mixerIndex = 2;
    public AudioSource songSource;
    bool songLoaded;
    public AudioClip song;

    public float fadeTime = 0.5f;
	// Use this for initialization
	void Start () {
        ac = FindObjectOfType<AudioController>();
        songSource = ac.GetComponent<AudioSource>();
        ac.FadeOutAudio(mixerIndex, false, fadeTime);
	}
	
	// Update is called once per frame
	void Update () {
        if (!ac.IsFading(mixerIndex) && !songLoaded)
        {
            songLoaded = true;
            songSource.clip = song;
            ac.FadeOutAudio(mixerIndex, true, fadeTime);
        }
	}
}
