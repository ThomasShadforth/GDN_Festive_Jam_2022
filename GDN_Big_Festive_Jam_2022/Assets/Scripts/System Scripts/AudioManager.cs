using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _musicAudioSource;
    [SerializeField] AudioSource _SFXAudioSource;

    [SerializeField] Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            if (s.name.Contains("FX"))
            {
                s.source = _SFXAudioSource;

            }
            else
            {
                s.source = _musicAudioSource;
            }

            s.source.clip = s.clip;
            s.source.playOnAwake = false;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name) {
        
        
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            return;
        }

        s.source.Play();

    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            return;
        }

        s.source.Stop();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
