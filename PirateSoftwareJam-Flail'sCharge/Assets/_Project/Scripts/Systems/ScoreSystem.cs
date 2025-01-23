using UnityEngine;
using UnityEngine.Serialization;

public class ScoreSystem : MonoBehaviour
{

    public static ScoreSystem instance;

    [Header("Score")]
    [FormerlySerializedAs("_score")] public float score;

    [Header("Score Settings")]
    [SerializeField] float killScoreIncrease;

    [SerializeField] float damageScoreIncrease;

    private void Start()
    {
        instance = this;
    }
    void OnEnable()
    {
        score = 0;
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

    void UpdateHUDScore(){
        HUD.instance.UpdateScoreText(score);
    }
}