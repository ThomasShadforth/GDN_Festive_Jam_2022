using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixerGroup masterAudioMixer;
    public AudioMixerGroup musicAudioMixer;
    public AudioMixerGroup soundEffectAudioMixer;

    public void SetMasterAudioMixer(float volume)
    {
        masterAudioMixer.audioMixer.SetFloat("Master_Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicAudioMixer(float volume)
    {
        musicAudioMixer.audioMixer.SetFloat("Music_Volume", Mathf.Log10(volume) * 20);
    }

    public void SetSoundAudioMixer(float volume)
    {
        soundEffectAudioMixer.audioMixer.SetFloat("FX_Volume", Mathf.Log10(volume) * 20);
    }
}
