using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;


public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;

    [Header ("Debug")]
    [SerializeField] bool isPaused = false;

    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isAttacking = false;
    [SerializeField] public int Health;
    [SerializeField] bool isDead = false;
    //[SerializeField] float timerCD = 0;

    [Header ("References")]
    [SerializeField] Rigidbody playerBody;

    [SerializeField] GameObject playerAvatar;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Animator animator;
    [SerializeField] Health healthScript;
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] PlayerInputActions playerInputActions;
    [SerializeField] PlayerInputHandler playerInputHandler;

    [Header ("Settings")]
    [SerializeField] float movementSpeed;

    [SerializeField] float attackCD;

    void Awake () {
        Debug.Log ("Awake");
        Instance = this;
        isDead = false;
        if (playerInput == null) {
            playerInput = new PlayerInput ();
        }

        if (playerInputHandler == null) {
            playerInputHandler = new PlayerInputHandler ();
        }

        if (playerInputActions == null) {
            playerInputActions = PlayerInputHandler.playerInputs;
        }
    }

    public void OnEnable () {
        PlayerInputHandler.Enable ();
        PlayerInputHandler.OnMovePerformed.AddListener (InputMove);
        PlayerInputHandler.OnAttackPerformed.AddListener (OnAttack);
    }

    public void OnDisable () {
        PlayerInputHandler.OnMovePerformed.RemoveListener (InputMove);
        PlayerInputHandler.OnAttackPerformed.RemoveListener (OnAttack);
    }

    public void OnDestroy () { }

    void Start () {
        healthScript = GetComponent<Health> ();
        healthScript.ResetHealth ();
        Health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
    }

    void Update () {
        if (moveInput != Vector2.zero) {
            OnPlayerMove ();
        }
    }

    void InputMove (Vector2 _input) {
        moveInput = _input;
        if (moveInput != Vector2.zero && !isAttacking) {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }

    void OnPlayerMove () {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.up;
        Vector3 moveCombined = new Vector3 (moveInput.x, 0, moveInput.y);
        if (moveCombined != Vector3.zero && !isAttacking) {
            playerBody.linearVelocity = new Vector3 (moveDirection.x, 0, moveDirection.y) * movementSpeed;
        } else {
            playerBody.linearVelocity = Vector3.zero;
        }
    }

    void OnAttack (bool _attacking) {
        if (_attacking && !isAttacking) {
            StartCoroutine (AttackLimit ());
        }
    }

    IEnumerator AttackLimit () {
        isAttacking = true;
        Debug.Log ("Player Attack");
        yield return new WaitForSeconds (attackCD);
        isAttacking = false;
    }
}