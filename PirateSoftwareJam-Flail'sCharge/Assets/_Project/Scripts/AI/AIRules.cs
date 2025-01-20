using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AIRules : MonoBehaviour {
    [Header ("References")]
    [SerializeField] string aiName;

    [SerializeField] public Transform aiTransform;
    [SerializeField] public Rigidbody aiRigidBody;
    [SerializeField] public Collider aiCollider;
    public ActionObject currentAction;

    void Awake () {
        ValidateAndInitActionObject ();
    }

    void OnEnable () {
        currentAction?.OnEnable ();
    }

    void OnDisable () {
        currentAction?.OnDisable ();
    }

    void Start () {
        RegisterAndConfigureActionObject ();
        currentAction?.Start ();
    }

    void Update () { }

    void FixedUpdate () {
        currentAction?.FixedUpdate ();
    }

    void OnDrawGizmos () {
        currentAction?.OnDrawGizmos ();
    }

    void ValidateAndInitActionObject () {
        if (currentAction == null) {
            Debug.Log ($"{gameObject.name} disabled");
            gameObject.SetActive (false);
            return;
        }

        currentAction = Instantiate (currentAction);
        Debug.Log ($"{gameObject.name} {currentAction.GetInstanceID ()} instantiated");
        currentAction?.Awake ();
    }

    void RegisterAndConfigureActionObject () {
        if (currentAction != null) { }
    }

    void OnCollisionEnter (Collision other) { }
}