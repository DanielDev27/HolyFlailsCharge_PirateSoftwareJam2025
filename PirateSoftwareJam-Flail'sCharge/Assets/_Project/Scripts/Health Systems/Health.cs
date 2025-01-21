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

    [Header ("Damage Cooldown")]
    [HideInInspector] public bool OnCooldown = false;

    public float damageCooldownTime;
    bool _coroutineActivated = false;

    public bool isDying = false;
    public bool IsInvincible = false;

    public static UnityEvent onDeath;
    public static UnityEvent<int> onHit;

    [SerializeField] bool Player;
    [SerializeField] PlayerController playerController;
    [SerializeField] AIScript aiScript;

    void Awake () {
        //Fill hp to its max
        currentHp = maxHp;
        OnCooldown = false;
        _coroutineActivated = false;
    }

    void OnEnable () { }

    public void TakeDamage (int damageAmount) //Take damage, if hp is below or equal to 0 - start death coroutine
    {
        if (OnCooldown) return;
        if (IsInvincible) return;
        if (isDying) return;


        currentHp -= (damageAmount);
        //onHit.Invoke (currentHp);
        StartCoroutine (DamageCooldown ());

        if (currentHp <= 0 && isDying == false) //Activate death coroutine if hp is below or equal to 0 and if entity isnt already dying
        {
            StartCoroutine (DeathRoutine ());
        }
    }

    public void ResetHealth () {
        currentHp = maxHp;
    }

    /*public void SetMaxHealth (int value) {
        maxHp = value;
    }*/

    IEnumerator DeathRoutine () //Wait for X seconds and then destroy entity
    {
        isDying = true;
        yield return new WaitForSeconds (timeBeforeDeath);
        if (Player) {
            onDeath.Invoke ();
        } else {
            //EnemyAI death function
        }
    }

    public IEnumerator DamageCooldown () //Use this to give Invulnerability frames if needed.
    {
        if (!_coroutineActivated) {
            _coroutineActivated = true;
            OnCooldown = true;

            yield return new WaitForSeconds (damageCooldownTime);

            OnCooldown = false;
            _coroutineActivated = false;
        }
    }

    void OnTriggerEnter (Collider other) {
        if (Player) {
            playerController = this.GetComponentInParent<PlayerController> ();
            if (playerController.isAttacking && other.gameObject.GetComponent<AIScript> () != null) {
                Debug.Log ("Hit Enemy");
                aiScript = other.GetComponentInParent<AIScript> ();
                aiScript.TakeHit (damage);
                //playerController.weaponTrigger.GetComponent<Collider> ().enabled = false;
            }
        } else {
            aiScript = this.GetComponentInParent<AIScript> ();
            if (aiScript.isAttacking && other.gameObject.GetComponent<PlayerController> () != null) {
                Debug.Log ("Hit Player");
                playerController = other.gameObject.GetComponent<PlayerController> ();
                playerController.TakeHit (damage);
                aiScript.weaponTrigger.GetComponent<Collider> ().enabled = false;
            }
        }
    }
}