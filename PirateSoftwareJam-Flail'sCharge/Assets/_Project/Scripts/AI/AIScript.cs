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
    [SerializeField] float timerCD;

    [Header ("References")]
    [SerializeField] Health healthScript;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [Header ("Settings")]
    [SerializeField] float moveSpeed;

    [SerializeField] int maxHealth;
    [SerializeField] float attackCD;

    void Awake () {
        Instance = this;
        coroutineInProgress = false;
        isDead = false;
        player = FindObjectOfType<PlayerController> ();
        playerReference = player?.gameObject;
    }

    void Update () {
        if (!coroutineInProgress) {
            switch (currentState) {
                case AiStates.Idle:
                    Debug.Log ("Idle");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    Debug.Log ("Chasing");
                    OnAnimatorUpdate ();
                    Chasing ();
                    break;
                case AiStates.Attacking:
                    Debug.Log ("Attacking");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnAttck ());
                    break;
                case AiStates.Dead:
                    Debug.Log ("Dead");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnDead ());
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
        if (playerReference != null) {
            distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
        }

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
        coroutineInProgress = false;
    }

    void Chasing () { }

    IEnumerator OnAttck () {
        yield return new WaitForSeconds (1);
    }

    IEnumerator OnDead () {
        yield return new WaitForSeconds (1);
    }

    void EnemyDie () { }

    IEnumerator EnemyDespawn () {
        yield return new WaitForSeconds (1);
    }
}

public enum AiStates //AI States for Logic
{
    Idle,
    Chasing,
    Attacking,
    Dead,
}