using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreSystem : MonoBehaviour
{

    public static ScoreSystem instance;

    public bool isGameOver = false;

    [Header("Score")]
    [FormerlySerializedAs("_score")] public float score;
    int numberOfWaves;

    [Header("Score Settings")]
    [SerializeField] float killScoreIncrease;

    [SerializeField] float damageScoreIncrease;

    [Header("Score UI")]

    [SerializeField] CanvasGroup gameEndCanvasGroup;
    [SerializeField] TMP_Text finalScoreText;

    private void Start()
    {
        instance = this;
    }
    void OnEnable()
    {
        numberOfWaves = 0;
        score = 0;
        isGameOver = false;
    }

    public void AddScoreKill()
    {
        score += killScoreIncrease;
        Debug.Log("Score increased to: " + score);
    }

    public void AddScoreDamage()
    {
        score += damageScoreIncrease;
        Debug.Log("Score increased to: " + score);
    }

    void UpdateHUDScore()
    {
        HUD.instance.UpdateScoreText(score);
    }
    public void TriggerGameEnd()
    {
        gameEndCanvasGroup.alpha = 1;
        gameEndCanvasGroup.interactable = true;
        gameEndCanvasGroup.blocksRaycasts = true;
        isGameOver = true;
        finalScoreText.text = "Your Score is " + score + Environment.NewLine + " You survived for " + numberOfWaves + "waves.";
    }
    public void IncreaseWaveCounter()
    {
        numberOfWaves += 1;
    }

}