using System.Collections;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] ballSounds;
    public Sound[] ballFISounds;
    public Sound[] UISounds;
    public Sound[] music;
    public Sound[] ambientSounds;
    public Sound[] miscSounds;

    Sound currentLvlSound;
    Sound nextLvlSound;
    Sound currentMusic;
    Sound nextMusic;

    float t1, t2, t3;
    bool fade2LvlSound = false;
    bool clearLvlSounds = false;
    float time4SoundFade = 1.5f;
    float time4MusicFade = 2.5f;
    bool fade2Music = false;
    bool clearMusic = false;
    bool go2Basement = false;
    bool comeBackFromBasement = false;

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

        for (int i = 0; i < ballSounds.Length; i++)
        {
            ballSounds[i].source = gameObject.AddComponent<AudioSource>();
            ballSounds[i].source.clip = ballSounds[i].clip;

            ballSounds[i].source.volume = ballSounds[i].volume;
            ballSounds[i].source.pitch = ballSounds[i].pitch;
            ballSounds[i].source.loop = ballSounds[i].loop;
            ballSounds[i].source.ignoreListenerVolume = ballSounds[i].ignoreListenerVolume;
            ballSounds[i].source.ignoreListenerPause = ballSounds[i].ignoreListenerPause;
            ballSounds[i].index = i;

            for (int n = 0; n < 4; n++)
            {
                AudioSource s = gameObject.AddComponent<AudioSource>();
                s.clip = ballSounds[i].clip;

                s.volume = ballSounds[i].volume;
                s.pitch = ballSounds[i].pitch;
                s.loop = ballSounds[i].loop;
                s.ignoreListenerVolume = ballSounds[i].ignoreListenerVolume;
                s.ignoreListenerPause = ballSounds[i].ignoreListenerPause;

                ballSounds[i].backUpSources.Enqueue(s);
            }
        }
        for (int i = 0; i < ballFISounds.Length; i++)
        {
            ballFISounds[i].source = gameObject.AddComponent<AudioSource>();
            ballFISounds[i].source.clip = ballFISounds[i].clip;

            ballFISounds[i].source.volume = ballFISounds[i].volume;
            ballFISounds[i].source.pitch = ballFISounds[i].pitch;
            ballFISounds[i].source.loop = ballFISounds[i].loop;
            ballFISounds[i].source.ignoreListenerVolume = ballFISounds[i].ignoreListenerVolume;
            ballFISounds[i].source.ignoreListenerPause = ballFISounds[i].ignoreListenerPause;

            ballFISounds[i].index = i;
        }
        for (int i = 0; i < UISounds.Length; i++)
        {
            UISounds[i].source = gameObject.AddComponent<AudioSource>();
            UISounds[i].source.clip = UISounds[i].clip;

            UISounds[i].source.volume = UISounds[i].volume;
            UISounds[i].source.pitch = UISounds[i].pitch;
            UISounds[i].source.loop = UISounds[i].loop;
            UISounds[i].source.ignoreListenerVolume = UISounds[i].ignoreListenerVolume;
            UISounds[i].source.ignoreListenerPause = UISounds[i].ignoreListenerPause;
        }
        for (int i = 0; i < music.Length; i++)
        {
            music[i].source = gameObject.AddComponent<AudioSource>();
            music[i].source.clip = music[i].clip;

            music[i].source.volume = music[i].volume;
            music[i].source.pitch = music[i].pitch;
            music[i].source.loop = music[i].loop;
            music[i].source.ignoreListenerVolume = music[i].ignoreListenerVolume;
            music[i].source.ignoreListenerPause = music[i].ignoreListenerPause;
        }
        for (int i = 0; i < ambientSounds.Length; i++)
        {
            ambientSounds[i].source = gameObject.AddComponent<AudioSource>();
            ambientSounds[i].source.clip = ambientSounds[i].clip;

            ambientSounds[i].source.volume = ambientSounds[i].volume;
            ambientSounds[i].source.pitch = ambientSounds[i].pitch;
            ambientSounds[i].source.loop = ambientSounds[i].loop;
            ambientSounds[i].source.ignoreListenerVolume = ambientSounds[i].ignoreListenerVolume;
            ambientSounds[i].source.ignoreListenerPause = ambientSounds[i].ignoreListenerPause;
        }
        for (int i = 0; i < miscSounds.Length; i++)
        {
            miscSounds[i].source = gameObject.AddComponent<AudioSource>();
            miscSounds[i].source.clip = miscSounds[i].clip;

            miscSounds[i].source.volume = miscSounds[i].volume;
            miscSounds[i].source.pitch = miscSounds[i].pitch;
            miscSounds[i].source.loop = miscSounds[i].loop;
            miscSounds[i].source.ignoreListenerVolume = miscSounds[i].ignoreListenerVolume;
            miscSounds[i].source.ignoreListenerPause = miscSounds[i].ignoreListenerPause;
        }
    }

    public void Fade2LvlSound(string lvlSoundName)
    {
        try
        {
            Sound s = Array.Find(ambientSounds, sound => sound.name == lvlSoundName);
            s.source.volume = 0;
            s.source.Play();

            t1 = 0;

            nextLvlSound = s;

            fade2LvlSound = true;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + lvlSoundName + " not found!");
            return;
        }
    }

    public void Fade2Music(string musicName)
    {
        try
        {
            Sound s = Array.Find(music, sound => sound.name == musicName);
            s.source.volume = 0;
            s.source.Play();

            t2 = 0;

            nextMusic = s;

            fade2Music = true;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + musicName + " not found!");
            return;
        }
    }

    private void Update()
    {
        if (fade2LvlSound)
        {
            t1 += Time.deltaTime / time4SoundFade;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlSound.volume, 0, t1);
            nextLvlSound.source.volume = Mathf.Lerp(0, nextLvlSound.volume, t1);

            if (currentLvlSound.source.volume == 0 && nextLvlSound.source.volume == nextLvlSound.volume)
            {
                currentLvlSound.source.Stop();
                currentLvlSound = nextLvlSound;
                nextLvlSound = null;

                fade2LvlSound = false;
            }
        }

        if (clearLvlSounds)
        {
            t1 += Time.deltaTime / time4SoundFade;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlSound.volume, 0, t1);

            if (currentLvlSound.source.volume == 0)
            {
                currentLvlSound.source.Stop();
                currentLvlSound = null;

                clearLvlSounds = false;
            }
        }

        if (fade2Music)
        {
            t2 += Time.deltaTime / time4MusicFade;

            if (currentMusic!=null)
            {
                currentMusic.source.volume = Mathf.Lerp(currentMusic.volume, 0, t2);

                nextMusic.source.volume = Mathf.Lerp(0, nextMusic.volume, t2);

                if (currentMusic.source.volume == 0 && nextMusic.source.volume == nextMusic.volume)
                {
                    currentMusic.source.Stop();
                    currentMusic = nextMusic;
                    nextMusic = null;

                    fade2Music = false;
                }
            }
            else
            {
                nextMusic.source.volume = Mathf.Lerp(0, nextMusic.volume, t2);
                if (nextMusic.source.volume == nextMusic.volume)
                {
                    currentMusic = nextMusic;
                    nextMusic = null;

                    fade2Music = false;
                }
            }
        }

        if (clearMusic)
        {
            t2 += Time.deltaTime / time4MusicFade;

            currentMusic.source.volume = Mathf.Lerp(currentMusic.volume, 0, t2);
            if (currentMusic.source.volume == 0)
            {
                currentMusic.source.Stop();
                currentMusic = null;

                clearMusic = false;
            }
        }

        if (go2Basement)
        {
            t3 += Time.deltaTime;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlSound.volume, currentLvlSound.volume - .15f, t3);

            if (currentLvlSound.source.volume == currentLvlSound.volume - .15f)
            {
                go2Basement = false;
            }
        }

        if (comeBackFromBasement)
        {
            t3 += Time.deltaTime;

            currentLvlSound.source.volume = Mathf.Lerp(currentLvlSound.volume - .15f, currentLvlSound.volume, t3);

            if (currentLvlSound.source.volume == currentLvlSound.volume)
            {
                comeBackFromBasement = false;
            }
        }
    }

    public void PlayLvlSound(string lvlSoundName)
    {
        try
        {
            if (currentLvlSound != null)
            {
                currentLvlSound.source.Stop();
            }
            currentLvlSound = Array.Find(ambientSounds, sound => sound.name == lvlSoundName);

            currentLvlSound.source.volume = currentLvlSound.volume;
            currentLvlSound.source.Play();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + lvlSoundName + " not found!");
            return;
        }
    }

    public void ClearLvlSounds()
    {
        t1 = 0;
        clearLvlSounds = true;
    }

    public void StopLvlSounds()
    {
        if (currentLvlSound != null)
        {
            currentLvlSound.source.Stop();
        }
    }

    public void PlayMusic(string musicName, bool stop = false)
    {
        try
        {
            currentMusic = Array.Find(music, sound => sound.name == musicName);

            currentMusic.source.volume = currentMusic.volume;
            currentMusic.source.Play();
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + musicName + " not found!");
            return;
        }
    }

    public void ClearMusic()
    {
        t2 = 0;
        clearMusic = true;
    }

    public void StopMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.source.Stop();
            currentMusic = null;
        }
    }

    public int BallImpactSound(string regImpactSoundName)
    {
        return Array.Find(ballSounds, sound => sound.name == regImpactSoundName).index;
    }

    public int BallFISound(string FISoundName)
    {
        return Array.Find(ballFISounds, sound => sound.name == FISoundName).index;
    }

    public void PlayBallSound(int index)
    {
        try
        {
            Sound s = ballSounds[index];
            if (s.source.isPlaying)
            {
                AudioSource a = s.backUpSources.Dequeue();
                s.source = a;
                s.source.Play();
                s.backUpSources.Enqueue(a);
            }
            else
            {
                s.source.Play();
            }

        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + index + " not found!");
            return;
        }
    }

    public void PlayBallFISound(int index)
    {
        try
        {
            ballFISounds[index].source.Play();
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + index + " not found!");
            return;
        }
    }

    public void PlayUISound(string name, bool stop = false)
    {
        try
        {
            if (!stop)
            {
                Array.Find(UISounds, sound => sound.name == name).source.Play();
            }
            else
            {
                Array.Find(UISounds, sound => sound.name == name).source.Stop();
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
    }

    public void PlayAmbientSound(string name, bool stop = false)
    {
        try
        {
            if (!stop)
            {
                Array.Find(ambientSounds, sound => sound.name == name).source.Play();
            }
            else
            {
                Array.Find(ambientSounds, sound => sound.name == name).source.Stop();
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
    }

    public void PlayMiscSound(string name, bool stop = false)
    {
        try
        {
            if (!stop)
            {
                Array.Find(miscSounds, sound => sound.name == name).source.Play();
            }
            else
            {
                Array.Find(miscSounds, sound => sound.name == name).source.Stop();
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }
    }

    public void Go2LabBasement()
    {
        comeBackFromBasement = false;

        t3 = 0;
        go2Basement = true;
    }

    public void ComeBackFromBasement()
    {
        go2Basement = false;

        t3 = 0;
        comeBackFromBasement = true;
    }

    public void LetCurrentMusicIgnoreFadeOut(bool yes)
    {
        if (currentMusic != null)
        {
            if (yes)
                currentMusic.source.ignoreListenerVolume = true;
            else
                currentMusic.source.ignoreListenerVolume = false;
        }
    }
}
