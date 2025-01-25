using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
 public enum EnemyVariant 
{
    Goblin,
    Wolf,
    Mage,
    Orc
}
public class AIScript : MonoBehaviour {
    public static AIScript Instance;

    [Header ("Debug")]
    //Player Reference Info
    [SerializeField] PlayerController player;

    [SerializeField] public GameObject playerReference;
    [SerializeField] public Vector3 directionToPlayer;

    [SerializeField] float distanceToPlayer;
    [SerializeField] bool isPaused;

    //AI Info
    [SerializeField] private EnemyVariant enemyVariant;
    public AiStates currentState;
    public AiStates returnState;
    [SerializeField] bool coroutineInProgress;
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isAttacking;
    [SerializeField] public bool isDead;
    [SerializeField] bool canSeePlayer;
    [SerializeField] int health;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackCD;
    //[SerializeField] float timerCD;

    [Header ("References")]
    [SerializeField] public Enemy enemySO;

    [SerializeField] public Health healthScript;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] public GameObject weaponTrigger;
    [SerializeField] GameObject projectile;
    [SerializeField] AudioSource audioSource;


    [Header ("Settings")]
    [SerializeField] float weaponReach;

    [SerializeField] bool ranged;
    [SerializeField] LayerMask obstructionLayerMask;

    [Header ("Score Script")]
    [SerializeField] ScoreSystem scoreSystemScript;
    


    void Awake () {
        Instance = this;
        //Player Reference
        player = FindFirstObjectByType<PlayerController> ();
        playerReference = player?.gameObject;
        //Health Script Reference
        healthScript = GetComponent<Health> ();
        healthScript.ResetHealth ();
        //AI
        coroutineInProgress = false;
        isDead = false;
        health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
        isPaused = false;
        //Scriptable Object Information Pull
        moveSpeed = enemySO.EnemyMoveSpeed;
    }

    void OnEnable () {
        MenuFunctionality.OnPause.AddListener (OnPause);
    }

    void OnPause (bool _isPaused) {
        isPaused = _isPaused;
        if (isPaused) {
            returnState = currentState;
        }

        if (!isPaused) {
            currentState = returnState;
        }
    }

    void Update () {
        if (playerReference != null) {
            distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
        }

        //Switch for current AI State
        if (!coroutineInProgress) {
            switch (currentState) {
                case AiStates.Idle:
                    //Debug.Log ("Idle {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing {" + this.gameObject.name + "}");
                    OnAnimatorUpdate ();
                    Chasing ();
                    break;
                case AiStates.Attacking:
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

        if (ranged) { VisionCheck (); }
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

    void VisionCheck () {
        if (playerReference != null && !isDead) {
            RaycastHit _hit;
            Vector3 _playerPosition = playerReference.transform.position;
            directionToPlayer = _playerPosition - transform.position;
            bool _hitLayer = Physics.Raycast (transform.position, directionToPlayer, out _hit, Mathf.Infinity, obstructionLayerMask, QueryTriggerInteraction.Ignore);
            if (_hitLayer && _hit.collider.gameObject.layer == 7) {
                transform.LookAt (new Vector3 (playerReference.transform.position.x, transform.position.y, playerReference.transform.position.z));
                Debug.DrawRay (transform.position + transform.up * 0.6f, directionToPlayer * weaponReach, Color.blue);
                canSeePlayer = true;
            } else {
                Debug.DrawRay (transform.position + transform.up * 0.6f, directionToPlayer * weaponReach, Color.red);
                canSeePlayer = false;
            }
        }
    }

    //Idle State Logic
    IEnumerator OnIdle () {
        if (!isPaused) {
            //Pause If statement
            {
                coroutineInProgress = true;
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
            }
        } //Pause if End

        else {
            currentState = AiStates.Idle;
            isMoving = false;
            isAttacking = false;
            agent.isStopped = true;
        }

        coroutineInProgress = false;
    }

    //Chasing Logic
    void Chasing () {
        if (playerReference != null && !isDead && !isAttacking && !isPaused) {
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

        if (playerReference == null || isPaused) {
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
        if (!isDead && !isPaused) {
            Debug.Log ("Attacking {" + this.gameObject.name + "}");
            coroutineInProgress = true;
            isAttacking = true;
            if (!ranged) {
                weaponTrigger.GetComponent<Collider> ().enabled = true;
                yield return new WaitForSeconds (attackCD);
                isAttacking = false;
                coroutineInProgress = false;
                weaponTrigger.GetComponent<Collider> ().enabled = false;
            }

            if (ranged) {
                if (canSeePlayer) {
                    //
                    //Generate Projectile
                    GameObject _projectile = Instantiate (projectile, weaponTrigger.transform.position, Quaternion.identity, transform);
                    _projectile.transform.forward = new Vector3 (playerReference.transform.position.x, transform.position.y, playerReference.transform.position.z);
                    //
                    yield return new WaitForSeconds (attackCD);
                    isAttacking = false;
                    coroutineInProgress = false;
                } else {
                    isAttacking = false;
                    currentState = AiStates.Chasing;
                    coroutineInProgress = false;
                }
            }

            currentState = distanceToPlayer > weaponReach ? AiStates.Chasing : AiStates.Attacking;
        } else if (isDead) {
            currentState = AiStates.Dead;
        } else if (isPaused) {
            currentState = AiStates.Idle;
        }
    }

    //Damage
    public void TakeHit (int damage) {
        healthScript.TakeDamage (damage);
        UpdateHealth ();
        ScoreSystem.instance?.AddScoreDamage ();
        switch (enemyVariant)
        {
            case EnemyVariant.Goblin:
                AudioManager.PlaySound((int)SoundType.HURTGOBLIN);
                break;
            case EnemyVariant.Wolf:
                AudioManager.PlaySound((int)SoundType.HURTWOLF);
                break;
            case EnemyVariant.Orc:
                AudioManager.PlaySound((int)SoundType.HURTORC);
                break;
            case EnemyVariant.Mage:
                //AudioManager.PlaySound((int)SoundType.HURTMAGE);
                break;
        }
    }

    //Health Update
    void UpdateHealth () {
        health = healthScript.currentHp;
        isDead = healthScript.isDying;
    }

    public void EnemyDie () {
        ScoreSystem.instance?.AddScoreKill ();
        StartCoroutine (OnDead ());
        audioSource.Stop();
    }

    IEnumerator OnDead () {
        coroutineInProgress = true;
        //yield return new WaitForSeconds (1);
        //Death Animation
        yield return new WaitForSeconds (1);
        //this.gameObject.SetActive (false); //why not destroy the gameObject?
        Destroy (this.gameObject);
    }
}

public enum AiStates //AI States for Logic
{
    Idle,
    Chasing,
    Attacking,
    Dead,
}