using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixerGroup masterAudioMixer;
    public AudioMixerGroup musicAudioMixer;
    public AudioMixerGroup soundEffectAudioMixer;

    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _SFXSlider;

    public void SetMasterAudioMixer(float volume)
    {
        masterAudioMixer.audioMixer.SetFloat("Master_Volume", Mathf.Log10(volume) * 20f);
    }

    public void SetMusicAudioMixer(float volume)
    {
        musicAudioMixer.audioMixer.SetFloat("Music_Volume", Mathf.Log10(volume) * 20f);
    }

    public void SetSoundAudioMixer(float volume)
    {
        soundEffectAudioMixer.audioMixer.SetFloat("FX_Volume", Mathf.Log10(volume) * 20f);
    }

    public void SetAudioSliders()
    {
        _masterSlider.value = GetMixerVolume(masterAudioMixer);
        _musicSlider.value = GetMixerVolume(musicAudioMixer);
        _SFXSlider.value = GetMixerVolume(soundEffectAudioMixer);
    }

    float GetMixerVolume(AudioMixerGroup mixerToGet)
    {
        float tempVol = 0f;
        bool hasVal = masterAudioMixer.audioMixer.GetFloat("Master_Volume", out tempVol);

        if (hasVal)
        {
            tempVol /= 20f;
            tempVol = Mathf.Pow(10, tempVol);
        }

        return tempVol;
    }
}
