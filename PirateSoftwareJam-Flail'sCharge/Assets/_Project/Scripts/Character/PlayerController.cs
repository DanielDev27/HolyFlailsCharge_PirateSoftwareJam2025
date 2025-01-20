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

    public void OnDestroy () {
        throw new NotImplementedException ();
    }

    void InputMove (Vector2 arg0) {
        throw new NotImplementedException ();
    }

    void OnAttack (bool arg0) {
        throw new NotImplementedException ();
    }
}