using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioAgent : MonoBehaviour {

    public void Start()
    {
    }

    [System.Serializable]
    public class AudioSet
    {
        public string audioTag;
        public AudioSource aSource;
        public AudioMixerGroup amg;
        public AudioClip[] audioClips;
        public float minPitch = 1;
        public float maxPitch = 2;

        public void PlayRandomClip()
        {
            if (aSource && audioClips.Length > 0 )
            {
                aSource.pitch = Random.Range(minPitch, maxPitch);
                aSource.outputAudioMixerGroup = amg ? amg : aSource.outputAudioMixerGroup;
                aSource.clip = audioClips [Random.Range(0, audioClips.Length)];
                aSource.Play();
            }
        }
    }


    public AudioSet[] audioSets;
    // Use this for initialization
    public void PlayAudioTag(string tagToPlay)
    {
        foreach (AudioSet audioSet in audioSets)
        {
            if (audioSet.audioTag == tagToPlay)
            {
                audioSet.PlayRandomClip();
                return;
            }
        }

        Debug.Log("no audio found for audio tag: " + tagToPlay);
    }
}
