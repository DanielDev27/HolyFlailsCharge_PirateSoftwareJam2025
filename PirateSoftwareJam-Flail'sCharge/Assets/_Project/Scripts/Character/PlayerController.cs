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
    [SerializeField] public bool isAttacking = false;
    [SerializeField] int health;
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
    [SerializeField] public GameObject weaponTrigger;

    [Header ("Settings")]
    [SerializeField] float movementSpeed;

    [SerializeField] float attackCD;

    void Awake () {
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

        healthScript = GetComponentInChildren<Health> ();
        healthScript.ResetHealth ();
        health = healthScript.maxHp;
        attackCD = healthScript.damageCooldownTime;
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
        weaponTrigger.GetComponent<Collider> ().enabled = true;
        isAttacking = true;
        //Debug.Log ("Player Attack");
        yield return new WaitForSeconds (attackCD);
        isAttacking = false;
        weaponTrigger.GetComponent<Collider> ().enabled = false;
    }

    public void TakeHit (int damage) {
        healthScript.TakeDamage (damage);
        UpdateHealth ();
    }

    void UpdateHealth () {
        health = healthScript.currentHp;
    }
}