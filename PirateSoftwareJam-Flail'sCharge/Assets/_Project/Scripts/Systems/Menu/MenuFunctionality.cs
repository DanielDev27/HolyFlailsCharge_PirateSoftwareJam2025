using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctionality : MonoBehaviour {
    [Header ("Main Menu")]
    [Header ("Pause Menu")]
    [SerializeField] Canvas pauseCanvas;

    [SerializeField] bool isPaused = false;

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

    public void InputPause (bool pause) {
        isPaused = pause;
        pauseCanvas.enabled = isPaused;
    }

    public void ResumeGame () {
        isPaused = false;
        pauseCanvas.enabled = isPaused;
    }
}