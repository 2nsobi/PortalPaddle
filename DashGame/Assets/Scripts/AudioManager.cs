using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;

    Sound currentLvlSound;
    Sound nextLvlSound;

    float t;
    float currentLvlVolumeMax;
    float nextLvlvolumeMax;
    bool fade2LvlSound;
    bool clearLvlSounds;
    float time4Fade = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            sounds[i].source.clip = sounds[i].clip;

            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
            sounds[i].source.loop = sounds[i].loop;
        }
    }

    public void Fade2LvlSound(string lvlSoundName)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == lvlSoundName);
            s.source.volume = 0;
            s.source.Play();

            t = 0;

            nextLvlvolumeMax = s.volume;
            nextLvlSound = s;

            fade2LvlSound = true;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + lvlSoundName + " not found!");
            return;
        }
    }

    private void Update()
    {
        if (fade2LvlSound)
        {
            t += Time.deltaTime / time4Fade;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlVolumeMax, 0, t);
            nextLvlSound.source.volume = Mathf.Lerp(0, nextLvlvolumeMax, t);

            if(currentLvlSound.source.volume == 0 && nextLvlSound.source.volume == nextLvlvolumeMax)
            {
                currentLvlSound.source.Stop();
                currentLvlSound = nextLvlSound;
                currentLvlVolumeMax = nextLvlvolumeMax;
                nextLvlSound = null;

                fade2LvlSound = false;
            }
        }

        if (clearLvlSounds)
        {
            t += Time.deltaTime / time4Fade;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlVolumeMax, 0, t);

            if(currentLvlSound.source.volume == 0)
            {
                currentLvlSound.source.Stop();
                currentLvlSound = null;

                clearLvlSounds = false;
            }
        }
    }

    public void PlayLvlSound(string lvlSoundName)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == lvlSoundName);

            currentLvlSound = s;
            currentLvlVolumeMax = s.volume;

            s.source.volume = s.volume;
            s.source.Play();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + lvlSoundName + " not found!");
            return;
        }
    }

    public void ClearLvlSounds()
    {
        t = 0;
        clearLvlSounds = true;
    }

    public void StopLvlSounds()
    {
        if (currentLvlSound != null)
        {
            currentLvlSound.source.Stop();
        }
    }

    public void Play(string name)
    {
        try
        {
            Array.Find(sounds, sound => sound.name == name).source.Play();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
    }
}
