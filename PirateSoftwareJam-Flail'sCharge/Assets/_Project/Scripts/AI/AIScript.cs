using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour {
    public static AIScript Instance;

    [Header ("Debug")]
    //Player Reference Info
    [SerializeField] PlayerController player;

    [SerializeField] public GameObject playerReference;
    [SerializeField] public Vector3 directionToPlayer;

    [SerializeField] float distanceToPlayer;

    //AI Info
    public AiStates currentState;
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

    [SerializeField] float attackCD;
    [SerializeField] float weaponReach;

    [Header ("Score Script")]
    [SerializeField] ScoreSystem scoreSystemScript;

    void Awake () {
        Instance = this;
        //Player Reference
        player = FindObjectOfType<PlayerController> ();
        playerReference = player?.gameObject;
        //Health Script Reference
        healthScript = GetComponentInChildren<Health> ();
        healthScript.ResetHealth ();
        //Score Script Reference
        scoreSystemScript = FindFirstObjectByType<ScoreSystem> ();
        //AI
        coroutineInProgress = false;
        isDead = false;
        health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
    }

    void Update () {
        if (playerReference != null) {
            distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
        }

        //Switch for current AI State
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

    //Animator update from AI State
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

//Idle State Logic
    IEnumerator OnIdle () {
        coroutineInProgress = true;

//Pause If statement
        {
            if (!isDead && !isAttacking) {
                yield return new WaitForSeconds (1);
                if (playerReference != null) {
                    //Logic for no player
                    currentState = AiStates.Chasing;
                    coroutineInProgress = false;
                } else {
                    currentState = AiStates.Idle;
                }
            } else {
                //Set state to Dead
                if (isDead) {
                    currentState = AiStates.Dead;
                }

                if (isAttacking) {
                    //Set state to Attacking
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

//Chasing Logic
    void Chasing () {
        if (playerReference != null && !isDead && !isAttacking) {
            //Player present
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
        }

        if (playerReference == null) {
            agent.isStopped = true;
            isMoving = false;
            currentState = AiStates.Idle;
        }

        if (isDead) {
            agent.isStopped = true;
            isMoving = false;
            currentState = AiStates.Dead;
        }
    }

//Attack Logic
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

    //Damage
    public void TakeHit (int damage) {
        healthScript.TakeDamage (damage);
        UpdateHealth ();
        scoreSystemScript?.AddScoreDamage ();
    }

    //Health Update
    void UpdateHealth () {
        health = healthScript.currentHp;
        isDead = healthScript.isDying;
    }

    public void EnemyDie () {
        scoreSystemScript?.AddScoreKill ();
        StartCoroutine (OnDead ());
    }

    IEnumerator OnDead () {
        coroutineInProgress = true;
        yield return new WaitForSeconds (1);
        this.gameObject.SetActive (false); //why not destroy the gameObject?
                                           //-Destroying prefabs causes errors
                                           //-Potential for Object Pooling
    }
}

public enum AiStates //AI States for Logic
{
    Idle,
    Chasing,
    Attacking,
    Dead,
}