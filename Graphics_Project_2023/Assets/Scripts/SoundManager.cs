using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;

    public const string MAIN_MENU_CHAR_SELECTION_MUSIC = "Main Menu";
    public const string FREE_ROAM_MUSIC = "Free Roam";
    public const string COMBAT_MODE_MUSIC = "Combat Mode";
    public const string DEFEAT_MUSIC = "Defeat";
    public const string VICTORY_MUSIC = "Victory";

    /* actions music */
    public const string WALKING_MUSIC = "Walking";
    public const string ATTACK_MUSIC = "Attack";
    public const string PLAY_MUSIC = "Play Music";
    public const string HEAL_MUSIC = "Heal";
    public const string BEG_MUSIC = "Beg";
    public const string LEVEL_UP = "Level Up";
    public const string NO_ACTION = "Unsuccessful Action";

    /* UI Sounds */
    public const string BUTTON_PRESS = "Button Press";

    //private const string 

    public Sound[] soundsArray;

    private void Awake() {
        /* Implement the singleton pattern in order to have only one instance of the object */
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        /* Do not Destroy the Object when we change scenes */
        DontDestroyOnLoad(gameObject);

        /* For each sound add an audio source */
        foreach (Sound sound in soundsArray) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip; // sound that willl actually play

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLooping;
        }
    }

    /* Play the free Roam Sound from the start */
    private void Start() {
        StartCoroutine(this.PlaySound(MAIN_MENU_CHAR_SELECTION_MUSIC));
        //SoundManager.Instance.PlaySoundWithoutFade(SoundManager.WALKING_MUSIC);
    }


    int stopFader;
    int startFader;
    private void Update() {
        if (stopFader < 50) {
            stopFader++;
        }
        if (startFader < 50) {
            startFader++;
        }
    }


    /* Create a method that will be called to play a specific sound */
    public IEnumerator PlaySound (string name) {
        Sound soundToPlay = Array.Find(soundsArray, sound => sound.soundName == name); // find the sound with this specific name at the sounds array
        if (soundToPlay == null) {
            Debug.Log("Sound Not Found");
            yield return null;
        }

        float startingVolume = soundToPlay.source.volume;
        soundToPlay.source.volume = 0f;
        soundToPlay.source.Play();   // play the sound clip

        float fadeTime = 5;
        while (soundToPlay.source.volume < startingVolume) {
            soundToPlay.source.volume += startingVolume / fadeTime;
            yield return new WaitWhile(() => startFader < 50);
            Debug.Log("Wait to start: Volume "+ soundToPlay.source.volume);
            Debug.Log("Start Volume: "+startingVolume);
            startFader = 0;
        }
        
    }

    /* Create a method that will be called to stop the play of a specific sound */
    /* This method fades out the stopping music */
    public IEnumerator StopSound(string name) {
        Sound soundToStop = Array.Find(soundsArray, sound => sound.soundName == name); // find the sound with this specific name at the sounds array
        if (soundToStop == null) {
            Debug.Log("Sound Not Found");
            yield return null; 
        }

        float startingVolume = soundToStop.source.volume;
        float fadeTime = 5f;
        while (soundToStop.source.volume > 0) {
            soundToStop.source.volume -= startingVolume / fadeTime;
            yield return new WaitWhile(() => stopFader < 50);
            Debug.Log("Waiting Volume: "+soundToStop.source.volume);
            stopFader = 0 ;
        }
        //yield return null; 
        soundToStop.source.Stop();   // stop the sound clip
        soundToStop.volume = startingVolume;
    }

    /* Same function without fade */
    public void PlaySoundWithoutFade(string name) {
        Sound soundToPlay = Array.Find(soundsArray, sound => sound.soundName == name); // find the sound with this specific name at the sounds array
        if (soundToPlay == null) {
            Debug.Log("Sound Not Found");
            return;
        }
        soundToPlay.source.Play();

    }

    public void StopSoundWithoutFade(string name) {
        Sound soundToStop = Array.Find(soundsArray, sound => sound.soundName == name); // find the sound with this specific name at the sounds array
        if (soundToStop == null) {
            Debug.Log("Sound Not Found");
            return;
        }
        soundToStop.source.Stop();
    }


}
