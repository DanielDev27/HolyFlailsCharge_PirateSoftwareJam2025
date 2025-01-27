using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    [Header ("HP Settings")]
    public int currentHp;

    public int maxHp;

    [Header ("Death Settings")]
    public bool slowmoOnDeath = false;

    public bool stopVinyl = false;

    //Time before an entity dies after its hp drops below 0
    public float timeBeforeDeath = 0f;
    public bool isDying = false;


    [Header ("Damage Cooldown")]
    //[HideInInspector] public bool OnCooldown = false;
    public float damageCooldownTime;

    bool _coroutineActivated = false;
    public bool IsInvincible = false;

    public static UnityEvent onDeath;
    public static UnityEvent<int> onHit;

    [Header ("References")]
    //Other Systems
    [SerializeField] ScoreSystem scoreSystemScript;

    [SerializeField] WaveSpawner waveSpawner;

    //Player
    [SerializeField] bool Player;

    [SerializeField] PlayerController playerController;

    //AI
    [SerializeField] AIScript aiScript;
    [SerializeField] HealthView healthView;

    void Awake () {
        //Fill hp to its max
        if (Player) {
            playerController = this.GetComponent<PlayerController> ();
        } else {
            aiScript = this.GetComponent<AIScript> ();
            healthView = this.GetComponentInChildren<HealthView> ();
            //Refactored to pull information from the SO
            maxHp = (int) aiScript.enemySO.EnemyMaxHealth;
            damageCooldownTime = aiScript.enemySO.EnemyAttackCD;
        }

        //OnCooldown = false;
        //_coroutineActivated = false;

        scoreSystemScript = FindFirstObjectByType<ScoreSystem> ();
        waveSpawner = FindFirstObjectByType<WaveSpawner> ();
    }

    void OnEnable () { IsInvincible = false; }

    void Start () {
        ResetHealth ();
        if (!Player) {
            healthView.UpdateHealthValue ((float) currentHp / (float) maxHp);
        }
    }

    public void ResetHealth () {
        currentHp = maxHp;
        if (Player) {
            HUD.instance?.UpdateDisplayedHealth (currentHp);
        }
    }

    public void TakeDamage (int damageAmount) //Take damage, if hp is below or equal to 0 - start death coroutine
    {
        if (Player) {
            if (!isDying) {
                currentHp -= (damageAmount);
            }

            HUD.instance?.UpdateDisplayedHealth (currentHp);
        } else {
            //if (OnCooldown) return;
            if (isDying) return;
            switch (IsInvincible) {
                case true:
                    return;
                case false:
                    IsInvincible = true;
                    StartCoroutine (DamageCooldown ());
                    currentHp -= (damageAmount);
                    healthView.UpdateHealthValue ((float) currentHp / (float) maxHp);
                    break;
            }
        }

        if (currentHp <= 0 && isDying == false) //Activate death coroutine if hp is below or equal to 0 and if entity isnt already dying
        {
            StartCoroutine (DeathRoutine ());
        }
    }


    public IEnumerator DamageCooldown () //Use this to give Invulnerability frames if needed.
    {
        if (!_coroutineActivated) {
            _coroutineActivated = true;
            yield return new WaitForSeconds (damageCooldownTime);
            IsInvincible = false;
            _coroutineActivated = false;
        }
    }

    IEnumerator DeathRoutine () //Wait for X seconds and then destroy entity
    {
        isDying = true;
        waveSpawner?.HandleEnemyDeath ();
        yield return new WaitForSeconds (timeBeforeDeath);
        if (Player) {
            playerController.Death ();
        }
    }
}