using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Enumeration for different sound types
// Each entry represents a type of sound that can be played
public enum SoundType {
    //UI     Moved this to the top so that I don't have to keep changing the numbers every time I add or remove a sound type
    MENUHOVER,
    MENUCLICK,
    VICTORY,
    DEFEAT,

    //Attacks
    FLAIL, 
    MAGIC,
    SWORD,
    BITE,

    //Damage
    HURTGOBLIN,
    HURTORC,
    HURTMAGE,
    HURTWOLF,
    HURTPLAYER,
    HURTFLESH,

    //Spawn
    SPAWNBOSS,
    SPAWNWAVE,
    SPAWNWOLFBOSS
}

[RequireComponent (typeof (AudioSource)), ExecuteInEditMode] // Ensures an AudioSource component is present and allows this script to run in edit mode
public class AudioManager : MonoBehaviour {
    [SerializeField] private SoundList[] soundList; // Array to store lists of sounds, each associated with a specific SoundType
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private List<AudioClip> musicTracks = new List<AudioClip>();
    [SerializeField] private float fadeTime = 1f; 
    private static AudioManager instance;
    [SerializeField] private AudioSource SFXAudioSource;
    

    private void Awake () {
        instance = this; // Set the static instance to this script instance
    }
    public void UpdateMusicForWave(int waveNumber)
    {
        AudioClip clipToPlay = GetClipForWave(waveNumber);
        
        // Only change the music if it's different from what's currently playing
        if (musicAudioSource.clip != clipToPlay)
        {
            StartCoroutine(FadeTrack(clipToPlay));
        }
    }
    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeElapsed = 0;
        float startVolume = musicAudioSource.volume;

        // Fade out current track
        while (timeElapsed < fadeTime){
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Change to new clip
        musicAudioSource.clip = newClip;
        musicAudioSource.Play();

        // Reset time elapsed for fade in
        timeElapsed = 0;

        // Fade in new track
        while (timeElapsed < fadeTime){
            musicAudioSource.volume = Mathf.Lerp(0, startVolume, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at exact target volume
        musicAudioSource.volume = startVolume;
    }
    private AudioClip GetClipForWave(int waveNumber)
    {
        // Return based on your specified wave rules
        if (waveNumber >= 1 && waveNumber <= 4)
            return musicTracks[0];  // Track 1: Waves 1-4
        else if (waveNumber == 5)
            return musicTracks[1];  // Track 2: Wave 5
        else if (waveNumber >= 6 && waveNumber <= 9)
            return musicTracks[2];  // Track 3: Waves 6-9
        else if (waveNumber == 10)
            return musicTracks[3];  // Track 4: Wave 10
        else if (waveNumber >= 11 && waveNumber <= 14)
            return musicTracks[4];  // Track 5: Waves 11-14
        else if (waveNumber == 15)
            return musicTracks[5];  // Track 6: Wave 15
        else if (waveNumber >= 16 && waveNumber <= 19)
            return musicTracks[6];  // Track 7: Waves 16-19
        else if (waveNumber == 20)
            return musicTracks[7];  // Track 8: Wave 20
        
        return musicTracks[0];
    }

    public static void PlaySound (int sound) {
        // Get the array of sound clips associated with the specified sound type
        AudioClip[] clips = instance.soundList[sound].Sounds;

        // Select a random sound clip from the array
        if (clips.Length > 1) {
            AudioClip randomClip = clips[UnityEngine.Random.Range (0, clips.Length)];
            instance.SFXAudioSource.PlayOneShot (randomClip);
        } else {
            AudioClip clip = clips[0];
            instance.SFXAudioSource.PlayOneShot (clip);
        }
    }

#if UNITY_EDITOR
    private void OnEnable () {
        string[] names = Enum.GetNames (typeof (SoundType));
        Array.Resize (ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++) {
            soundList[i].name = names[i];
        }
    }

#endif
}

// Serializable struct to represent a list of sounds associated with a specific SoundType
[Serializable]
public struct SoundList {
    public AudioClip[] Sounds { get => sounds; }
    [SerializeField] public string name;
    [SerializeField] private AudioClip[] sounds;
}