using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuFunctionality : MonoBehaviour {
    [Header ("Main Menu")]
    [SerializeField] Canvas menuCanvas;

    [SerializeField] Canvas creditsCanvas;

    [Header ("Pause Menu")]
    [SerializeField] Canvas pauseCanvas;

    [SerializeField] bool isPaused = false;

    //Event
    public static UnityEvent<bool> OnPause = new UnityEvent<bool> ();

    void Awake () {
        if (pauseCanvas != null) {
            pauseCanvas.enabled = false;
        }
    }

    void OnEnable () {
        PlayerInputHandler.Enable ();
        PlayerInputHandler.OnPausePerformed.AddListener (InputPause);
    }

    public void LoadLevel (int _levelIndex) {
        if (SceneManager.GetActiveScene ().buildIndex == _levelIndex) {
            return;
        }

        SceneManager.LoadScene (_levelIndex);
    }

    public void QuitGame () {
        Application.Quit ();
    }

    public void SeeCredits () {
        menuCanvas.enabled = false;
        creditsCanvas.enabled = true;
    }

    public void LeaveCredits () {
        creditsCanvas.enabled = false;
        menuCanvas.enabled = true;
    }

    public void InputPause (bool pause) {
        isPaused = pause;
        pauseCanvas.enabled = isPaused;
        OnPause.Invoke (isPaused);
    }

    public void ResumeGame () {
        isPaused = false;
        pauseCanvas.enabled = isPaused;
        OnPause.Invoke (isPaused);
    }

}