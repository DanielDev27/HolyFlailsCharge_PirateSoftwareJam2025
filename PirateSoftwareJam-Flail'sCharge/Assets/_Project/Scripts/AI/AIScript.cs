using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyVariant {
    Goblin,
    Wolf,
    Mage,
    Orc
}

public class AIScript : MonoBehaviour {
    public static AIScript Instance;
    static readonly int IsWalking = Animator.StringToHash ("IsWalking");
    static readonly int IsAttacking = Animator.StringToHash ("IsAttacking");
    static readonly int IsDead = Animator.StringToHash ("IsDead");
    static readonly int TakeDamage = Animator.StringToHash ("TakeDamage");

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
    [SerializeField] AnimationClip attackAnimation;


    [Header ("Settings")]
    [SerializeField] float weaponReach;

    [SerializeField] bool ranged;
    [SerializeField] LayerMask obstructionLayerMask;
    [SerializeField] public float playerHeightCorrection;
    [SerializeField] float visionCorrection;

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
        isPaused = MenuFunctionality.instance.isPaused;
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
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing {" + this.gameObject.name + "}");
                    Chasing ();
                    break;
                case AiStates.Attacking:
                    StartCoroutine (OnAttack ());
                    break;
                case AiStates.Dead:
                    Debug.Log ("Dead {" + this.gameObject.name + "}");
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
                animator.SetBool (IsWalking, isMoving);
                animator.SetBool (IsAttacking, isAttacking);
                break;
            case AiStates.Chasing:
                animator.SetBool (IsWalking, isMoving);
                animator.SetBool (IsAttacking, isAttacking);
                break;
            case AiStates.Attacking:
                animator.SetBool (IsWalking, isMoving);
                animator.SetBool (IsAttacking, isAttacking);
                break;
            case AiStates.Dead:
                animator.SetBool (IsWalking, isMoving);
                animator.SetBool (IsAttacking, isAttacking);
                animator.SetTrigger (IsDead);
                break;
        }
    }

    void VisionCheck () {
        if (playerReference != null && !isDead) {
            RaycastHit _hit;
            Vector3 _playerPosition = playerReference.transform.position;
            directionToPlayer = new Vector3 (_playerPosition.x - transform.position.x, _playerPosition.y - visionCorrection, _playerPosition.z - transform.position.z);
            bool _hitLayer = Physics.Raycast (weaponTrigger.transform.position, directionToPlayer, out _hit, Mathf.Infinity, obstructionLayerMask, QueryTriggerInteraction.Ignore);
            if (_hitLayer && _hit.collider.gameObject.layer == 7) {
                transform.LookAt (new Vector3 (playerReference.transform.position.x, transform.position.y, playerReference.transform.position.z));
                Debug.DrawRay (weaponTrigger.transform.position, directionToPlayer * weaponReach, Color.blue);
                canSeePlayer = true;
            } else {
                Debug.DrawRay (weaponTrigger.transform.position, directionToPlayer * weaponReach, Color.red);
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
                OnAnimatorUpdate ();
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
            FaceTarget ();
            isMoving = true;
            agent.speed = moveSpeed;
            agent.isStopped = false;
            //Reached Player
            if (distanceToPlayer <= weaponReach) {
                isMoving = false;
                agent.isStopped = true;
                currentState = AiStates.Attacking;
            }

            if (ranged && canSeePlayer && distanceToPlayer <= weaponReach) {
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

        OnAnimatorUpdate ();
    }

    /// <summary>
    /// Face Target script sourced from InsaneDuane @ https://discussions.unity.com/t/how-do-i-update-the-rotation-of-a-navmeshagent/750004/3
    /// Sets a significantly more immediate turn direction towards the next navmesh target.
    /// </summary>
    void FaceTarget () {
        var turnTowardNavSteeringTarget = agent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation (new Vector3 (direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    //Attack Logic
    IEnumerator OnAttack () {
        transform.LookAt (new Vector3 (playerReference.transform.position.x, transform.position.y, playerReference.transform.position.z));
        if (!isDead && !isPaused) {
            Debug.Log ("Attacking {" + this.gameObject.name + "}");
            coroutineInProgress = true;
            isAttacking = true;
            OnAnimatorUpdate ();
            if (!ranged) {
                DealDamageSound ();

                yield return new WaitForSeconds (attackAnimation.length - 0.2f);
                animator.SetBool (IsAttacking, false);
                yield return new WaitForSeconds (attackCD - attackAnimation.length);
                isAttacking = false;
                coroutineInProgress = false;
            }

            if (ranged) {
                if (canSeePlayer) {
                    //
                    DealDamageSound ();
                    yield return new WaitForSeconds (attackAnimation.length - 0.2f);
                    animator.SetBool (IsAttacking, false);
                    yield return new WaitForSeconds (attackCD - attackAnimation.length);
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

    public void AttackOn () {
        weaponTrigger.GetComponent<Collider> ().enabled = true;
    }

    public void AttackOff () {
        weaponTrigger.GetComponent<Collider> ().enabled = false;
    }

    public void GenerateProjectile () {
        if (canSeePlayer) {
            //Generate Projectile
            GameObject _projectile = Instantiate (projectile, weaponTrigger.transform.position, Quaternion.identity, transform);
            _projectile.transform.forward = new Vector3 (playerReference.transform.position.x, playerHeightCorrection, playerReference.transform.position.z);
            //
        }
    }

    //Damage
    public void TakeHit (int damage) {
        if (!healthScript.IsInvincible) {
            TakeDamageSound ();
            healthScript.TakeDamage (damage);
            UpdateHealth ();
            ScoreSystem.instance?.AddScoreDamage ();
            animator.SetTrigger (TakeDamage);
            if (!ranged) {
                AttackOff ();
            }
        }
    }

    //Sound pushes
    public void TakeDamageSound () {
        AudioManager.PlaySound ((int) SoundType.HURTFLESH);
        switch (enemyVariant) {
            case EnemyVariant.Goblin:
                AudioManager.PlaySound ((int) SoundType.HURTGOBLIN);
                break;
            case EnemyVariant.Wolf:
                AudioManager.PlaySound ((int) SoundType.HURTWOLF);
                break;
            case EnemyVariant.Orc:
                AudioManager.PlaySound ((int) SoundType.HURTORC);
                break;
            case EnemyVariant.Mage:
                AudioManager.PlaySound ((int) SoundType.HURTMAGE);
                break;
        }
    }

    public void DealDamageSound () {
        switch (enemyVariant) {
            case EnemyVariant.Goblin:
                AudioManager.PlaySound ((int) SoundType.SWORD);
                break;
            case EnemyVariant.Wolf:
                AudioManager.PlaySound ((int) SoundType.BITE);
                break;
            case EnemyVariant.Orc:
                AudioManager.PlaySound ((int) SoundType.SWORD);
                break;
            case EnemyVariant.Mage:
                AudioManager.PlaySound ((int) SoundType.MAGIC);
                break;
        }
    }


    //Health Update
    void UpdateHealth () {
        health = healthScript.currentHp;
        isDead = healthScript.isDying;
    }

    public void EnemyDie () {
        isDead = true;
        //currentState = AiStates.Dead;
        OnAnimatorUpdate ();
        ScoreSystem.instance?.AddScoreKill ();
        StartCoroutine (OnDead ());
        audioSource.Stop ();
    }

    IEnumerator OnDead () {
        coroutineInProgress = true;
        yield return new WaitForSeconds (1);
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