using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public static HUD instance;

    [Header ("Debug")]
    [SerializeField] float currentHp;

    [SerializeField] int waveCount;

    [Header ("HUD")]
    [SerializeField] Slider healthBar;

    [SerializeField] TMP_Text scoreText, waveText;


    void Awake () {
        instance = this;
        waveCount = 0;
    }

    public void UpdateDisplayedHealth (int _incomingNewHealth) {
        currentHp = _incomingNewHealth;
        healthBar.value = _incomingNewHealth;
    }

    public void UpdateScoreText (float _incomingScore) {
        scoreText.text = $"Score: {_incomingScore}";
    }

    public void IncrementWaveCount () {
        waveCount += 1;
        waveText.text = $"Wave: {waveCount}";
    }
}