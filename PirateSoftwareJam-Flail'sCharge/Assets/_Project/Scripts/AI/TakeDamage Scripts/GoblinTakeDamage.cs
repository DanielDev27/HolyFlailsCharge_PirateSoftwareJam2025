using UnityEngine;

public class GoblinTakeDamage : MonoBehaviour
{
    [SerializeField] public AudioManager audioManagerScript;
    void Start()
    {
        
    }

    void TakeDamage(){
        AudioManager.PlaySound((int)SoundType.HURTGOBLIN);
    }
}
