using System.Collections;
using System.Collections.Generic;
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
    public Sound[] otherGameModesMusic;

    Sound currentLvlSound;
    Sound nextLvlSound;
    Sound currentMusic;
    Sound nextMusic;
    Sound currentSong;

    AudioLowPassFilter audioLowPassFilter;
    AudioReverbFilter audioReverbFilter;

    Queue<Sound> OGGMRadioQ = new Queue<Sound>();
    int song2StartRadioWith; // the track you hear first changes each time you leave and enter a new game mode, could be same game mode
    Coroutine OGGMRadio;

    Coroutine stopMusicDelay;
    System.Random rng = new System.Random();

    float t1, t2, t3, t4;
    bool fade2LvlSound = false;
    bool clearLvlSounds = false;
    float time4SoundFade = 1.5f;
    float time4MusicFade = 3;
    float time4MuffleSound = 0.4f;
    bool fade2Music = false;
    bool clearMusic = false;
    bool go2Basement = false;
    bool comeBackFromBasement = false;
    bool noMusic;
    bool fadeOut;
    bool dontFadeSoundIn = false;
    bool muffleSound = false;
    bool fadeInMuffle;
    string musicAbout2End; //name of music that is about to end in stopMusicDelay() coroutine
    bool noSound;
    float audioLowPassCutOffFreq = 1500;

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
            ballSounds[i].source.bypassListenerEffects = ballSounds[i].bypassListenerEffects;
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
                s.bypassListenerEffects = ballSounds[i].bypassListenerEffects;

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
            ballFISounds[i].source.bypassListenerEffects = ballFISounds[i].bypassListenerEffects;

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
            UISounds[i].source.bypassListenerEffects = UISounds[i].bypassListenerEffects;
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
            music[i].source.bypassListenerEffects = music[i].bypassListenerEffects;
        }
        for (int i = 0; i < otherGameModesMusic.Length; i++)
        {
            otherGameModesMusic[i].source = gameObject.AddComponent<AudioSource>();
            otherGameModesMusic[i].source.clip = otherGameModesMusic[i].clip;

            otherGameModesMusic[i].source.volume = otherGameModesMusic[i].volume;
            otherGameModesMusic[i].source.pitch = otherGameModesMusic[i].pitch;
            otherGameModesMusic[i].source.loop = otherGameModesMusic[i].loop;
            otherGameModesMusic[i].source.ignoreListenerVolume = otherGameModesMusic[i].ignoreListenerVolume;
            otherGameModesMusic[i].source.ignoreListenerPause = otherGameModesMusic[i].ignoreListenerPause;
            otherGameModesMusic[i].source.bypassListenerEffects = otherGameModesMusic[i].bypassListenerEffects;
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
            ambientSounds[i].source.bypassListenerEffects = ambientSounds[i].bypassListenerEffects;

            //since these sound are streaming it is best to not stop and start them but instead change their volume when needed
            ambientSounds[i].source.volume = 0;
            ambientSounds[i].source.Play();
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
            miscSounds[i].source.bypassListenerEffects = miscSounds[i].bypassListenerEffects;
        }
    }

    private void Start()
    {
        noMusic = PlayerPrefsX.GetBool("noMusic");
        noSound = PlayerPrefsX.GetBool("noSound");
    }

    public void Fade2LvlSound(string lvlSoundName)
    {
        try
        {
            Sound s = Array.Find(ambientSounds, sound => sound.name == lvlSoundName);
            s.source.volume = 0;

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
        if (noMusic)
        {
            return;
        }
        try
        {
            Sound s = Array.Find(music, sound => sound.name == musicName);
            s.source.volume = 0;

            t2 = 0;

            nextMusic = s;

            dontFadeSoundIn = s.dontFadeMeIn;

            fadeOut = true;

            if (currentMusic == null)
            {
                nextMusic.source.Play();
            }
            else
            {
                if (!currentMusic.source.isPlaying)
                    currentMusic = null;
            }

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
                currentLvlSound = null;

                clearLvlSounds = false;
            }
        }

        if (fade2Music)
        {
            t2 += Time.deltaTime / time4MusicFade;

            if (currentMusic != null)
            {
                if (fadeOut)
                {
                    currentMusic.source.volume = Mathf.Lerp(currentMusic.volume, 0, t2);

                    if (currentMusic.source.volume == 0 && !nextMusic.source.isPlaying)
                    {
                        t2 = 0;

                        fadeOut = false;

                        nextMusic.source.PlayDelayed(1.4f);
                    }
                }
                else
                {
                    if (!dontFadeSoundIn)
                    {
                        nextMusic.source.volume = Mathf.Lerp(0, nextMusic.volume, t2);

                        if (nextMusic.source.volume == nextMusic.volume)
                        {
                            currentMusic.source.Stop();
                            currentMusic = nextMusic;
                            nextMusic = null;

                            fade2Music = false;
                        }
                    }
                    else
                    {
                        nextMusic.source.volume = nextMusic.volume;

                        currentMusic.source.Stop();
                        currentMusic = nextMusic;
                        nextMusic = null;

                        fade2Music = false;
                    }
                }
            }
            else
            {
                if (!dontFadeSoundIn)
                {
                    nextMusic.source.volume = Mathf.Lerp(0, nextMusic.volume, t2);
                    if (nextMusic.source.volume == nextMusic.volume)
                    {
                        currentMusic = nextMusic;
                        nextMusic = null;

                        fade2Music = false;
                    }
                }
                else
                {
                    nextMusic.source.volume = nextMusic.volume;

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

        // applying the lowpassfilter before other audio effects like reverb prevents noticeable crackling
        if (muffleSound)
        {
            t4 += Time.deltaTime / time4MuffleSound;
            if (fadeInMuffle)
            {
                audioLowPassFilter.cutoffFrequency = Mathf.Lerp(22000, audioLowPassCutOffFreq, t4);
                //ApplyReverb();

                if (audioLowPassFilter.cutoffFrequency <= audioLowPassCutOffFreq)
                {
                    muffleSound = false;
                    audioReverbFilter.reverbPreset = AudioReverbPreset.Psychotic;
                    audioReverbFilter.reverbPreset = AudioReverbPreset.User; // have to make the preset user before you can change any of the filters values
                    audioReverbFilter.reverbLevel = 0; //this prevents static
                    print(audioReverbFilter.reverbLevel);
                }
            }
            else
            {
                audioLowPassFilter.cutoffFrequency = Mathf.Lerp(audioLowPassCutOffFreq, 22000, t4);
                //RemoveReverb();

                if (audioLowPassFilter.cutoffFrequency >= 22000)
                {
                    muffleSound = false;
                    audioReverbFilter.reverbPreset = AudioReverbPreset.Off;
                }

                time4MuffleSound = 0.4f; //this line is called slighlty less if placed here instead of in mufflesound()
            }
        }
    }

    public void PlayLvlSound(string lvlSoundName)
    {
        try
        {
            if (currentLvlSound != null)
            {
                currentLvlSound.source.volume = 0;
            }
            currentLvlSound = Array.Find(ambientSounds, sound => sound.name == lvlSoundName);

            currentLvlSound.source.volume = currentLvlSound.volume;
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
            currentLvlSound.source.volume = 0;
        }
    }

    public void PlayMusic(string musicName, bool stop = false)
    {
        if (noMusic)
        {
            return;
        }
        try
        {
            currentMusic = Array.Find(music, sound => sound.name == musicName);

            if (currentMusic.name == musicAbout2End)
            {
                StopCoroutine(stopMusicDelay);
            }

            currentMusic.source.Play();
            currentMusic.source.volume = currentMusic.volume;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Sound " + musicName + " not found!");
            return;
        }
    }

    public void ClearMusic()
    {
        if (noMusic)
        {
            return;
        }
        t2 = 0;
        clearMusic = true;
    }

    public void StopMusic()
    {
        if (currentMusic != null)
        {
            stopMusicDelay = StartCoroutine(StopMusicDelay(currentMusic));
            currentMusic = null;
        }
    }

    IEnumerator StopMusicDelay(Sound music) //this prevents music crackling when stopping it since its streaming audio
    {
        music.source.volume = 0;
        musicAbout2End = music.name;
        yield return new WaitForSecondsRealtime(2);
        music.source.Stop();
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
                a.Play();
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

    public void SwitchUpRadioFirstSong()
    {
        if (noMusic)
        {
            return;
        }
        song2StartRadioWith = rng.Next(0, otherGameModesMusic.Length);
    }

    public void StartPlayingOGMMusicRadio()
    {
        if (noMusic)
        {
            return;
        }
        int j = song2StartRadioWith;
        for (int i = 0; i < otherGameModesMusic.Length; i++)
        {
            OGGMRadioQ.Enqueue(otherGameModesMusic[j]);

            j++;
            if (j == otherGameModesMusic.Length)
                j = 0;
        }

        song2StartRadioWith++;
        if (song2StartRadioWith == otherGameModesMusic.Length)
            song2StartRadioWith = 0;

        OGGMRadio = StartCoroutine(PlayNextSongInRadio());
    }

    IEnumerator PlayNextSongInRadio()
    {
        currentSong = OGGMRadioQ.Dequeue();
        OGGMRadioQ.Enqueue(currentSong);

        currentSong.source.Play();
        yield return new WaitForSeconds(currentSong.source.clip.length);

        OGGMRadio = StartCoroutine(PlayNextSongInRadio());
    }

    public void StopOGGMusicRadio()
    {
        if (noMusic)
        {
            return;
        }
        if (OGGMRadio != null)
            StopCoroutine(OGGMRadio);

        if (currentSong != null)
            currentSong.source.Stop();

        if (OGGMRadioQ.Count > 0)
            OGGMRadioQ.Clear();
    }

    public void Go2LabBasement()
    {
        comeBackFromBasement = false;

        t3 = 0;
        go2Basement = true;
    }

    public void ComeBackFromBasement()
    {
        PlayUISound("switchPageLouder");
        go2Basement = false;

        t3 = 0;
        comeBackFromBasement = true;
    }

    public void LetCurrentMusicIgnoreFadeOut(bool yes)
    {
        if (noMusic)
        {
            return;
        }
        if (currentSong != null)
        {
            if (yes)
                currentSong.source.ignoreListenerVolume = true;
            else
                currentSong.source.ignoreListenerVolume = false;
        }
    }

    public void SetNoMusic(bool noMusic)
    {
        this.noMusic = noMusic;
    }

    public void MuffleSound(bool yes, bool immediately = true)
    {
        if (!noSound)
        {
            if (yes)
            {
                t4 = 0;
                muffleSound = true;
                fadeInMuffle = true;
            }
            else
            {
                if (immediately)
                {
                    t4 = 0;
                    muffleSound = true;
                    fadeInMuffle = false;
                    time4MuffleSound = 0.2f;
                }
                else
                {
                    t4 = 0;
                    muffleSound = true;
                    fadeInMuffle = false;
                }
            }
        }
    }

    public void GetAudioFiltersFromCamera()
    {
        audioLowPassFilter = Camera.main.GetComponent<AudioLowPassFilter>();
        audioLowPassFilter.cutoffFrequency = 22000;
        audioReverbFilter = Camera.main.GetComponent<AudioReverbFilter>();
        TurnOffReverb();
    }

    public void TurnOffReverb()
    {
        audioReverbFilter.reverbPreset = AudioReverbPreset.Off;
    }

    //void ApplyReverb()
    //{
    //    audioReverbFilter.room = Mathf.Lerp(-10000, -1000, t4);
    //    audioReverbFilter.roomHF = Mathf.Lerp(-10000, -400, t4);
    //    audioReverbFilter.decayTime = Mathf.Lerp(1, 17.23f, t4);
    //    audioReverbFilter.decayHFRatio = Mathf.Lerp(1, 0.56f, t4);
    //    audioReverbFilter.reflectionsLevel = Mathf.Lerp(-2602, -1713, t4);
    //    audioReverbFilter.reverbLevel = Mathf.Lerp(200, -613, t4);
    //    audioReverbFilter.reverbDelay = Mathf.Lerp(0.011f, 0.03f, t4);
    //    audioReverbFilter.diffusion = Mathf.Lerp(0, 100, t4);
    //    audioReverbFilter.density = Mathf.Lerp(0, 100, t4);
    //}

    //void RemoveReverb()
    //{
    //    audioReverbFilter.room = Mathf.Lerp(-1000, -10000, t4);
    //    audioReverbFilter.roomHF = Mathf.Lerp(-400, -10000, t4);
    //    audioReverbFilter.decayTime = Mathf.Lerp(17.23f, 1, t4);
    //    audioReverbFilter.decayHFRatio = Mathf.Lerp(0.56f, 1, t4);
    //    audioReverbFilter.reflectionsLevel = Mathf.Lerp(-1713, -2602, t4);
    //    audioReverbFilter.reverbLevel = Mathf.Lerp(-613, 200, t4);
    //    audioReverbFilter.reverbDelay = Mathf.Lerp(0.03f, 0.011f, t4);
    //    audioReverbFilter.diffusion = Mathf.Lerp(100, 0, t4);
    //    audioReverbFilter.density = Mathf.Lerp(100, 0, t4);
    //}

    public void SetNoSound(bool noSound)
    {
        this.noSound = noSound;
    }
}
