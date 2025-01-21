using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour {
    public static AIScript Instance;

    [Header ("Debug")]
    [SerializeField] PlayerController player;

    [SerializeField] public GameObject playerReference;
    public AiStates currentState;
    [SerializeField] public Vector3 directionToPlayer;
    [SerializeField] float distanceToPlayer;
    [SerializeField] bool coroutineInProgress;

    [SerializeField] public bool isMoving;
    [SerializeField] public bool isAttacking;
    [SerializeField] public bool isDead;

    [SerializeField] int health;
    //[SerializeField] float timerCD;

    [Header ("References")]
    [SerializeField] Health healthScript;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] public GameObject weaponTrigger;


    [Header ("Settings")]
    [SerializeField] float moveSpeed;

    //[SerializeField] int maxHealth;
    [SerializeField] float attackCD;
    [SerializeField] float weaponReach;

    void Awake () {
        Instance = this;
        coroutineInProgress = false;
        isDead = false;
        player = FindObjectOfType<PlayerController> ();
        playerReference = player?.gameObject;
        healthScript = GetComponentInChildren<Health> ();
        healthScript.ResetHealth ();
        health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
    }

    void Update () {
        if (playerReference != null) {
            distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
        }

        if (!coroutineInProgress) {
            switch (currentState) {
                case AiStates.Idle:
                    Debug.Log ("Idle {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    Debug.Log ("Chasing {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    Chasing ();
                    break;
                case AiStates.Attacking:
                    Debug.Log ("Attacking {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnAttack ());
                    break;
                case AiStates.Dead:
                    Debug.Log ("Dead {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    EnemyDie ();
                    break;
            }
        }
    }

    void OnAnimatorUpdate () {
        switch (currentState) {
            case AiStates.Idle:
                break;
            case AiStates.Chasing:
                break;
            case AiStates.Attacking:
                break;
            case AiStates.Dead:
                break;
        }
    }

    IEnumerator OnIdle () {
        coroutineInProgress = true;

//Pause If statement
        {
            if (!isDead && !isAttacking) {
                yield return new WaitForSeconds (1);
                if (playerReference != null) {
                    currentState = AiStates.Chasing;
                    coroutineInProgress = false;
                } else {
                    currentState = AiStates.Idle;
                }
            } else {
                if (isDead) {
                    currentState = AiStates.Dead;
                }

                if (isAttacking) {
                    currentState = AiStates.Attacking;
                }
            }
        } //Pause if End
        /*else
        {
            currentAiState = AiStates.Idle;
            isMoving = false;
            isAttacking = false;
            agent.isStopped = true;
        }*/
        coroutineInProgress = false;
    }

    void Chasing () {
        if (playerReference != null && !isDead && !isAttacking) {
            agent.destination = playerReference.transform.position;
            isMoving = true;
            agent.speed = moveSpeed;
            agent.isStopped = false;
            //Reached Player
            if (distanceToPlayer <= weaponReach /*&& timerCD >= attackCD*/) {
                isMoving = false;
                agent.isStopped = true;
                currentState = AiStates.Attacking;
            }
        } else {
            isMoving = false;
            if (!isDead) {
                currentState = AiStates.Idle;
            } else {
                currentState = AiStates.Dead;
            }
        }
    }

    IEnumerator OnAttack () {
        if (!isDead) {
            coroutineInProgress = true;
            isAttacking = true;
            weaponTrigger.GetComponent<Collider> ().enabled = true;
            yield return new WaitForSeconds (attackCD);
            if (distanceToPlayer > weaponReach) {
                currentState = AiStates.Chasing;
            } else {
                currentState = AiStates.Attacking;
            }

            isAttacking = false;
            coroutineInProgress = false;
            weaponTrigger.GetComponent<Collider> ().enabled = false;
        } else {
            currentState = AiStates.Dead;
        }
    }

    public void TakeHit (int damage) {
        healthScript.TakeDamage (damage);
        UpdateHealth ();
    }

    void UpdateHealth () {
        health = healthScript.currentHp;
        isDead = healthScript.isDying;
    }

    public void EnemyDie () { StartCoroutine (OnDead ()); }

    IEnumerator OnDead () {
        coroutineInProgress = true;
        yield return new WaitForSeconds (1);
        StartCoroutine (EnemyDespawn ());
    }

    IEnumerator EnemyDespawn () {
        yield return new WaitForSeconds (1);
        this.gameObject.SetActive (false);
    }
}

public enum AiStates //AI States for Logic
{
    Idle,
    Chasing,
    Attacking,
    Dead,
}