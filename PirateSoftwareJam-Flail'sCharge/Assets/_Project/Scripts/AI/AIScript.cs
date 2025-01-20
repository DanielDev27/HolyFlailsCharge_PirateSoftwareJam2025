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
        playerReference = player.gameObject;
    }

    void Update () {
        if (!coroutineInProgress) {
            switch (currentState) {
                case AiStates.Idle:
                    OnAnimatorUpdate ();
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    OnAnimatorUpdate ();
                    Chasing ();
                    break;
                case AiStates.Attacking:
                    OnAnimatorUpdate ();
                    StartCoroutine (OnAttck ());
                    break;
                case AiStates.Dead:
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
        yield return new WaitForSeconds (1);
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