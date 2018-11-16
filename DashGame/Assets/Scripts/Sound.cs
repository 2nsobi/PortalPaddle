using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public AudioClip clip;

    public string name;
    [Range(0,1)]
    public float volume;
    [Range(-3, 3)]
    public float pitch;
    public bool loop;
    public bool ignoreListenerVolume;
    public bool ignoreListenerPause;
    public bool bypassListenerEffects;
    public bool dontFadeMeIn = false;
    [HideInInspector]
    public int index;

    [HideInInspector] //hides this variable since source is set in the awake method of the AudioManager
    public AudioSource source;
    [HideInInspector]
    public Queue<AudioSource> backUpSources = new Queue<AudioSource>();
}
