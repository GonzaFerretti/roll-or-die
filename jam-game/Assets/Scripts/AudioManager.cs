using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    public bool mute;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    

    private void Start()
    {
        mute = false;
    }

    public void Play(string soundName)
    {
        if (!mute)
        {
            Sound sound = Array.Find(sounds, sound => sound.name == soundName);

            if(sound == null)
            {
                Debug.Log("Error: AudioClip not found");
                
                return;
            }

            sound.source.Play();
        }
    }

    private void Toggle(){
        mute = !mute;

        if (mute)
        {
            Array.Find(sounds, sound => sound.name == "BattleMusic").source.Stop();
        } else {
            Array.Find(sounds, sound => sound.name == "BattleMusic").source.Play();
        }
    }

    private void Update(){
        if (Input.GetKeyDown("m"))
        {
            Toggle();
        }
    }
}
