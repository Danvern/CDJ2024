using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
public class FMODOptionsMenu : MonoBehaviour
{
    [Header("FMOD Event Buses")]
    [SerializeField] private string masterBusPath = "bus:/Master";
    [SerializeField] private string sfxBusPath = "bus:/SFX";
    [SerializeField] private string voiceBusPath = "bus:/VOC";
    [SerializeField] private string musicBusPath = "bus:/MUS";

    private Bus masterBus;
    private Bus sfxBus;
    private Bus voiceBus;
    private Bus musicBus;

    [Header("UI Elements")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        // Initialize FMOD Buses
        masterBus = RuntimeManager.GetBus(masterBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);
        voiceBus = RuntimeManager.GetBus(voiceBusPath);
        musicBus = RuntimeManager.GetBus(musicBusPath);

        // Load saved volume settings
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        voiceSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Apply the initial volume levels
        SetMasterVolume(masterSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetVoiceVolume(voiceSlider.value);
        SetMusicVolume(musicSlider.value);

        // Add listeners to the sliders
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void SetMasterVolume(float volume)
    {
        masterBus.setVolume(volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        sfxBus.setVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void SetVoiceVolume(float volume)
    {
        voiceBus.setVolume(volume);
        PlayerPrefs.SetFloat("VoiceVolume", volume);
    }

    private void SetMusicVolume(float volume)
    {
        musicBus.setVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Save the volume settings when the menu is closed or disabled
        PlayerPrefs.Save();
    }
}
