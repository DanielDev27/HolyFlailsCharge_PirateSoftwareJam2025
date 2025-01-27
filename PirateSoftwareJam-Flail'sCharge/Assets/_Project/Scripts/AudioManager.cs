using System;
using UnityEngine;


// Enumeration for different sound types
// Each entry represents a type of sound that can be played
public enum SoundType {
    //Attacks
    FLAIL, //0
    MAGIC,
    ARROW,
    SWORD,

    //Dammage
    HURTGOBLIN,
    HURTORC,
    HURTMAGE,
    HURTWOLF,

    //Spawn
    SPAWNBOSS,
    SPAWNWAVE,
    SPAWNWOLFBOSS,

    //UI
    MENUHOVER,
    MENUCLICK,
    VICTORY,
    DEFEAT,
}

[RequireComponent (typeof (AudioSource)), ExecuteInEditMode] // Ensures an AudioSource component is present and allows this script to run in edit mode
public class AudioManager : MonoBehaviour {
    [SerializeField] private SoundList[] soundList; // Array to store lists of sounds, each associated with a specific SoundType
    private static AudioManager instance;
    private AudioSource audioSource;

    private void Awake () {
        instance = this; // Set the static instance to this script instance
        audioSource = GetComponent<AudioSource> ();
    }

    public static void PlaySound (int sound) {
        // Get the array of sound clips associated with the specified sound type
        AudioClip[] clips = instance.soundList[sound].Sounds;

        // Select a random sound clip from the array
        if (clips.Length > 1) {
            AudioClip randomClip = clips[UnityEngine.Random.Range (0, clips.Length)];
            instance.audioSource.PlayOneShot (randomClip);
        } else {
            AudioClip clip = clips[0];
            instance.audioSource.PlayOneShot (clip);
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