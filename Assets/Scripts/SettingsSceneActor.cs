using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSceneActor : MonoBehaviour {

    public GameObject GameSettingsPanel;
    public GameObject SoundSettingsPanel;
    public GameObject GeneralSettingsPanel;

    public Slider markerSpeedSlider;
    public Slider cameraSpeedSlider;
    public Slider cursorSpeedSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioController audioController;

    GameObject panel;

    // Use this for initialization
    void Start() {
        audioController = FindObjectOfType<AudioController>();
        markerSpeedSlider.value = PlayerPrefs.GetFloat("MarkerSpeedPlayer1");
        cameraSpeedSlider.value = PlayerPrefs.GetFloat("CameraSpeedPlayer1");
        cursorSpeedSlider.value = PlayerPrefs.GetFloat("CursorSpeed");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffects");
    }

    // Update is called once per frame
    void Update() {

    }

    public void OpenGameSettingsPanel()
    {
        GameSettingsPanel.SetActive(true);
        SoundSettingsPanel.SetActive(false);
        GeneralSettingsPanel.SetActive(false);
    }

    public void OpenSoundSettingsPanel()
    {
        GameSettingsPanel.SetActive(false);
        SoundSettingsPanel.SetActive(true);
        GeneralSettingsPanel.SetActive(false);
    }

    public void OpenGeneralSettingsPanel()
    {
        GameSettingsPanel.SetActive(false);
        SoundSettingsPanel.SetActive(false);
        GeneralSettingsPanel.SetActive(true);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MarkerSpeedPlayer1", markerSpeedSlider.value);
        PlayerPrefs.SetFloat("CameraSpeedPlayer1", cameraSpeedSlider.value);
        PlayerPrefs.SetFloat("CursorSpeed", cursorSpeedSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SoundEffects", sfxVolumeSlider.value);
    }
}
