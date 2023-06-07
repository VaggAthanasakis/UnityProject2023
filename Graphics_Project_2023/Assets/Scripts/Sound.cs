using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]  // to appear on the editor
public class Sound {

    public AudioClip clip;

    public string soundName;
    public float volume;
    public float pitch;

    public bool isLooping;
    public bool isPlaying;

    public AudioSource source;

}
