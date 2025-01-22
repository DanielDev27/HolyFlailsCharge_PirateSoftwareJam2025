using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class PlayerInputHandler {
    public static PlayerInputActions playerInputs;

    //Events
    public static UnityEvent<Vector2> OnMovePerformed = new UnityEvent<Vector2> ();
    public static UnityEvent<bool> OnAttackPerformed = new UnityEvent<bool> ();
    public static UnityEvent<bool> OnPausePerformed = new UnityEvent<bool> ();

    //Values
    [ShowInInspector, ReadOnly] public static Vector2 moveInput;
    [ShowInInspector] public static bool attack = false;
    public static bool pause = false;

    static PlayerInputHandler instance;

    //Defining function for the PlayerInputHandler
    public static PlayerInputHandler Instance {
        get {
            if (instance == null) {
                instance = new PlayerInputHandler ();
            }

            return instance;
        }
        private set { instance = value; }
    }

    public static void Enable () {
        Debug.Log ("Enable PlayerInputHandler");
        if (playerInputs == null) {
            playerInputs = new PlayerInputActions ();
        }

        RegisterInputs ();
        playerInputs.Enable ();
    }

    public static void Disable () {
        if (playerInputs == null) {
            return;
        }

        playerInputs.Dispose ();
    }

    public static void Dispose () {
        if (playerInputs == null) {
            return;
        }

        playerInputs.Dispose ();
    }

    static void RegisterInputs () {
        //Move
        playerInputs.Player.Move.performed += MovePerformed;
        playerInputs.Player.Move.canceled += MovePerformed;
        //Attack
        playerInputs.Player.Attack.performed += AttackPerformed;
        playerInputs.Player.Attack.canceled += AttackCanceled;
        //Pause
        playerInputs.Player.Pause.performed += PausePerformed;
    }

//Movement Input Logic
    public static void MovePerformed (InputAction.CallbackContext ctx) {
        if (ctx.ReadValue<Vector2> ().normalized != Vector2.zero) {
            moveInput = ctx.ReadValue<Vector2> ().normalized;
        }

        if (ctx.ReadValue<Vector2> ().normalized == Vector2.zero) {
            moveInput = Vector2.zero;
        }

        OnMovePerformed?.Invoke (moveInput);
    }

//Attack Input Logic
    public static void AttackPerformed (InputAction.CallbackContext ctx) {
        attack = true;
        //Debug.Log ("Attack performed");
        OnAttackPerformed?.Invoke (attack);
    }

    public static void AttackCanceled (InputAction.CallbackContext ctx) {
        attack = false;
        //Debug.Log ("Attack canceled");
        OnAttackPerformed?.Invoke (attack);
    }

    //Pause Input Logic
    public static void PausePerformed (InputAction.CallbackContext ctx) {
        pause = true;
        OnPausePerformed?.Invoke (pause);
    }
}