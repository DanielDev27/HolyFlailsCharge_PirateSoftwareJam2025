using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [Header("ai script ref")]
    [SerializeField] AIScript aiScript;
    [SerializeField] float killScoreIncrease;
    [SerializeField] float damageScoreIncrease;
    private float _score;
    void Start(){
        aiScript = GetComponent<AIScript>();
        if (aiScript != null){
            Debug.Log("AIScript found!");
        }
    }

    void Update()
    {
        
    }

    public void AddScoreKill(){
        _score += killScoreIncrease;
        Debug.LogWarning("Score increased to: " + _score);
    }
    public void AddScoreDamage(){
        _score += damageScoreIncrease;
        Debug.LogWarning("Score increased to: " + _score);
    }
}
