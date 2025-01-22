using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    [Header ("HP Settings")]
    public int currentHp;

    public int maxHp;

    [Header ("Damage Settings")]
    [SerializeField] int damage;

    [Header ("Death Settings")]
    public bool slowmoOnDeath = false;

    public bool stopVinyl = false;

    //Time before an entity dies after its hp drops below 0
    public float timeBeforeDeath = 0f;
    public bool isDying = false;


    [Header ("Damage Cooldown")]
    //[HideInInspector] public bool OnCooldown = false;
    public float damageCooldownTime;

    //bool _coroutineActivated = false;
    //public bool IsInvincible = false;

    public static UnityEvent onDeath;
    public static UnityEvent<int> onHit;

    [SerializeField] ScoreSystem scoreSystemScript;

    [SerializeField] bool Player;
    [SerializeField] PlayerController playerController;
    [SerializeField] AIScript aiScript;

    void Awake () {
        //Fill hp to its max
        if (Player) {
            playerController = this.GetComponentInParent<PlayerController> ();
        } else {
            aiScript = this.GetComponentInParent<AIScript> ();
            //Refactored to pull information from the SO
            maxHp = (int) aiScript.enemySO.EnemyMaxHealth;
            damage = (int) aiScript.enemySO.EnemyDamage;
            damageCooldownTime = aiScript.enemySO.EnemyAttackCD;
        }

        currentHp = maxHp;
        //OnCooldown = false;
        //_coroutineActivated = false;

        scoreSystemScript = FindFirstObjectByType<ScoreSystem> ();
    }

    void OnEnable () { }

    public void TakeDamage (int damageAmount) //Take damage, if hp is below or equal to 0 - start death coroutine
    {
        if (Player && !isDying) {
            currentHp -= (damageAmount);
        } else {
            //if (OnCooldown) return;
            //if (IsInvincible) return;
            if (isDying) return;
            currentHp -= (damageAmount);
            //onHit.Invoke (currentHp);
            //StartCoroutine (DamageCooldown ());
        }

        if (currentHp <= 0 && isDying == false) //Activate death coroutine if hp is below or equal to 0 and if entity isnt already dying
        {
            StartCoroutine (DeathRoutine ());
        }
    }

    public void ResetHealth () {
        currentHp = maxHp;
    }

    IEnumerator DeathRoutine () //Wait for X seconds and then destroy entity
    {
        isDying = true;
        yield return new WaitForSeconds (timeBeforeDeath);
        if (Player) {
            playerController.Death ();
        }
    }

    /*public IEnumerator DamageCooldown () //Use this to give Invulnerability frames if needed.
    {
        if (!_coroutineActivated) {
            _coroutineActivated = true;
            OnCooldown = true;

            yield return new WaitForSeconds (damageCooldownTime);

            OnCooldown = false;
            _coroutineActivated = false;
        }
    }*/
    public float GetDamage () {
        return damage;
    }

//Damage trigger for entering a collider
    void OnTriggerEnter (Collider other) {
        if (Player) {
            playerController = this.GetComponentInParent<PlayerController> ();
            if (playerController.isAttacking && other.gameObject.GetComponent<AIScript> () != null) {
                //Debug.Log ("Hit Enemy");
                aiScript = other.GetComponentInParent<AIScript> ();
                aiScript.TakeHit (damage);
            }
        } else {
            if (aiScript.isAttacking && other.gameObject.GetComponent<PlayerController> () != null) {
                playerController = other.gameObject.GetComponent<PlayerController> ();
                playerController.TakeHit (damage);
            }
        }
    }
}