using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Debug")]
    [SerializeField] bool isPaused = false;

    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isMoving = false;
    [SerializeField] public bool isAttacking = false;
    [SerializeField] int health;
    [SerializeField] bool isDead = false;
    //[SerializeField] float timerCD = 0;

    [Header("References")]
    [SerializeField] Rigidbody playerBody;

    [SerializeField] GameObject playerAvatar;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Animator animator;
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] PlayerInputHandler playerInputHandler;
    [SerializeField] Health healthScript;
    [SerializeField] public GameObject weaponTrigger;

    [Header("Settings")]
    [SerializeField] float movementSpeed;

    [SerializeField] float attackCD;

    void Awake()
    {
        Instance = this;
        //Verify and Set PLayer Input script
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }

        if (playerInputHandler == null)
        {
            playerInputHandler = new PlayerInputHandler();
        }

        if (playerInputActions == null)
        {
            playerInputActions = PlayerInputHandler.playerInputs;
        }

        //Set values and references
        isDead = false;
        healthScript = GetComponent<Health>();
        healthScript.ResetHealth();
        health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
    }

    public void OnEnable()
    {
        PlayerInputHandler.Enable();
        PlayerInputHandler.OnMovePerformed.AddListener(InputMove);
        PlayerInputHandler.OnAttackPerformed.AddListener(OnAttack);
        MenuFunctionality.OnPause.AddListener(OnPause);
    }

    void OnPause(bool _isPaused)
    {
        isPaused = _isPaused;
        if (isPaused)
        {
            PlayerInputHandler.Disable();
        }
        else
        {
            PlayerInputHandler.Enable();
        }
    }


    public void OnDisable()
    {
        PlayerInputHandler.OnMovePerformed.RemoveListener(InputMove);
        PlayerInputHandler.OnAttackPerformed.RemoveListener(OnAttack);
    }

    public void OnDestroy() { }

    void Update()
    {
        if (moveInput != Vector2.zero)
        {
            OnPlayerMove(); //Get the player move input when it's not zero
        }
    }

    //Move Input and Debug bools
    void InputMove(Vector2 _input)
    {
        moveInput = _input;
        if (moveInput != Vector2.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    //Move Logic
    void OnPlayerMove()
    {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.up;
        Vector3 moveCombined = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveCombined != Vector3.zero)
        {
            playerBody.linearVelocity = new Vector3(moveDirection.x, 0, moveDirection.y) * movementSpeed;
        }
        else
        {
            playerBody.linearVelocity = Vector3.zero;
        }
    }

    //Attack Input
    void OnAttack(bool _attacking)
    {
        if (_attacking && !isAttacking)
        {
            StartCoroutine(AttackLimit());
        }
    }

    //Attack Logic and Debug
    IEnumerator AttackLimit()
    {
        weaponTrigger.GetComponent<Collider>().enabled = true; //Turn on Weapon collider
        isAttacking = true;
        //Debug.Log ("Player Attack");
        yield return new WaitForSeconds(attackCD);
        isAttacking = false;
        weaponTrigger.GetComponent<Collider>().enabled = false; ////Turn off Weapon collider
    }

    //Damage Function
    public void TakeHit(int damage)
    {
        Debug.Log("Hit Player");
        healthScript.TakeDamage(damage);
        UpdateHealth();
    }

    //Health Update Function
    void UpdateHealth()
    {
        health = healthScript.currentHp;
    }

    public void Death()
    {
        ScoreSystem.instance.TriggerGameEnd();
        isDead = true;
        OnDisable();

    }
}