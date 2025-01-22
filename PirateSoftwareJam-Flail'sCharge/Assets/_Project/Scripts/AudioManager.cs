using System;
using UnityEngine;
using UnityEngine.Audio;

// Enumeration for different sound types
// Each entry represents a type of sound that can be played
public enum SoundType
{
    FLAIL,
    MAGIC,
    ARROW,
    HURTGOBLIN,
    HURTORC,
    MENUCLICK,
    MENUHOVER,
    VICTORY,
    DEFEAT,
    SPAWNBOSS
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode] // Ensures an AudioSource component is present and allows this script to run in edit mode
public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList; // Array to store lists of sounds, each associated with a specific SoundType
    private static AudioManager instance; // Singleton instance of the AudioManager
    private AudioSource audioSource; // Reference to the AudioSource component

    // Called when the script instance is being loaded
    private void Awake(){
        instance = this; // Set the static instance to this script instance
        audioSource = GetComponent<AudioSource>(); // Cache the AudioSource component for playing sounds
    }

    // Static method to play a sound of the specified SoundType
    // Parameters:
    // - sound: The type of sound to play (from the SoundType enum)
    // - volume: The volume at which to play the sound (default is 1)
    public static void PlaySound(SoundType sound, float volume = 1){
        // Get the array of sound clips associated with the specified sound type
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;

        // Select a random sound clip from the array
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        // Play the selected sound clip at the specified volume
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

#if UNITY_EDITOR
    // Called when the script is enabled in the editor
    private void OnEnable()
    {
        Debug.Log("I have run in edit mode"); // Log a message to indicate that the script has run in edit mode

        // Get the names of all enum values in the SoundType enum
        string[] names = Enum.GetNames(typeof(SoundType));

        // Initialize the soundList array with the same length as the number of enum values
        soundList = new SoundList[names.Length];

        // Populate the soundList array with SoundList objects
        for (int i = 0; i < names.Length; i++) {
            // Create a new SoundList object and set its name to the corresponding enum value
            soundList[i] = new SoundList { name = names[i] };
        }
    }
#endif
}

// Serializable class to represent a list of sounds associated with a specific SoundType
[Serializable]
public class SoundList
{
    public string Name { get => name; set => name = value; } // Public property to get or set the name of the sound list
    public AudioClip[] Sounds { get => sounds; } // Public getter for the array of sound clips

    [SerializeField] public string name; // The name of the sound type (matches the SoundType enum)
    [SerializeField] private AudioClip[] sounds; // Array of sound clips associated with this sound type
}