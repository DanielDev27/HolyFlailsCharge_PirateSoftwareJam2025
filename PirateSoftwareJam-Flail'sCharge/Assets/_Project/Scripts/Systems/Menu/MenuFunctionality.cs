using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFunctionality : MonoBehaviour {
    [Header ("Main Menu")]
    [SerializeField] Canvas menuCanvas;

    [SerializeField] Canvas creditsCanvas;

    [Header ("Pause Menu")]
    [SerializeField] Canvas pauseCanvas;

    [SerializeField] bool isPaused = false;

    [Header ("Loading Screen")]
    public GameObject loadingScreen;

    public Image loadingProgressBar;
    [SerializeField] bool isActive;

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

        StartCoroutine (LoadingSceneAsync (_levelIndex));
    }

    IEnumerator LoadingSceneAsync (int sceneID) {
        loadingScreen?.SetActive (true);
        yield return new WaitForSeconds (0.1f);
        AsyncOperation _operation = SceneManager.LoadSceneAsync (sceneID);
        while (!_operation.isDone) {
            float progress = Mathf.Clamp01 (_operation.progress / 1f);
            loadingProgressBar.fillAmount = progress;
            yield return null;
        }

        Time.timeScale = 1f;
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