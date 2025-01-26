using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] AIScript aiScript;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform target;
    [SerializeField] Health healthScript;
    [SerializeField] Damage damageScript;
    [SerializeField] float damage;
    [SerializeField] float pSpeed;

    void Awake () {
        rb = GetComponent<Rigidbody> ();
    }

    void Start () {
        aiScript = GetComponentInParent<AIScript> ();
        target = FindAnyObjectByType<PlayerController> ().transform;
        healthScript = aiScript.healthScript;
        damageScript = aiScript.GetComponentInChildren<Damage> ();
        damage = damageScript.GetDamage ();
        transform.parent = null;
        transform.LookAt (target);
        rb.linearVelocity = new Vector3 (target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z) * pSpeed;
    }

    void OnTriggerEnter (Collider other) {
        if (other.gameObject.GetComponent<PlayerController> () != null) {
            other.gameObject.GetComponent<PlayerController> ().TakeHit ((int) damage);
            Destroy (this.gameObject);
        } else {
            StartCoroutine (ProjectileDestroy ());
        }
    }

    IEnumerator ProjectileDestroy () {
        yield return new WaitForSeconds (2f);
        Destroy (this.gameObject);
    }
}