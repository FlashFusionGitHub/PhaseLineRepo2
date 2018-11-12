using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class MyAudioMixerGroup
{
    public AudioMixerGroup m_audioMixerGroup;
    public float m_volume;
    public bool m_isFading;
    public float m_fadeTime;
    public AudioSource aS;
    public bool fadingIn;

    public void Update()
    {
        if (m_isFading)
        {
            if (fadingIn)
            {
                m_volume += Time.deltaTime / m_fadeTime;
                if (m_volume >= 1)
                {
                    m_isFading = false;
                }
            }
            else
            {
                m_volume -= Time.deltaTime / m_fadeTime;
                if (m_volume <= 0)
                {
                    m_isFading = false;
                }
            }

            if (aS)
            {
                aS.volume = m_volume;
            }
            else
            {
                m_audioMixerGroup.audioMixer.SetFloat(m_audioMixerGroup.name, m_volume);
            }
            
        }
    }
}

public class AudioController : MonoBehaviour {

    public AudioMixer audioMixer;
    public MyAudioMixerGroup[] my_audioMixerGroups;
    float lifeTime;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this.gameObject);
        foreach (AudioController ac in FindObjectsOfType<AudioController>())
        {
            if (lifeTime < ac.lifeTime)
            {
                Destroy(this.gameObject);
            }
        }
        lifeTime += Time.deltaTime;
        audioMixer.SetFloat(my_audioMixerGroups[0].m_audioMixerGroup.name, PlayerPrefs.GetFloat("MusicVolume"));
        audioMixer.SetFloat(my_audioMixerGroups[1].m_audioMixerGroup.name, PlayerPrefs.GetFloat("SoundEffects"));
        audioMixer.SetFloat(my_audioMixerGroups[2].m_audioMixerGroup.name, PlayerPrefs.GetFloat("UIVolume"));
    }
	
	// Update is called once per frame
	void Update () {

        lifeTime += Time.deltaTime;
        foreach (MyAudioMixerGroup my_amg in my_audioMixerGroups)
        {
            my_amg.Update();
        }
    }

    public void SetVolumes(int index, float volume)
    {
        audioMixer.SetFloat(my_audioMixerGroups[index].m_audioMixerGroup.name, volume);
    }

    public void FadeOutAudio(int index, bool isFading, float fadeTime)
    {
        my_audioMixerGroups[index].m_fadeTime = fadeTime;
        my_audioMixerGroups[index].m_isFading = true;
        my_audioMixerGroups[index].fadingIn = isFading;
    }

    public bool IsFading(int index)
    {
        return my_audioMixerGroups[index].m_isFading;
    }

}
