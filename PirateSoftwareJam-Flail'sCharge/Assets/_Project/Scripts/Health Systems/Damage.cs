using UnityEngine;
using System;

public class Damage : MonoBehaviour {
    [Header ("References")]
    [SerializeField] bool Player;

    [SerializeField] PlayerController playerController;
    [SerializeField] AIScript aiScript;

    [Header ("Settings")]
    [SerializeField] int damage;
    //Damage trigger for entering a collider

    public int GetDamage () {
        return damage;
    }

    void Awake () {
        //Fill hp to its max
        if (Player) {
            playerController = this.GetComponentInParent<PlayerController> ();
        } else {
            aiScript = this.GetComponentInParent<AIScript> ();
            damage = (int) aiScript.enemySO.EnemyDamage;
        }

        //OnCooldown = false;
        //_coroutineActivated = false;
    }

    void OnTriggerEnter (Collider other) {
        if (Player) {
            if (playerController.isAttacking && other.gameObject.GetComponent<AIScript> () != null) {
                //Debug.Log ("Hit Enemy");
                aiScript = other.GetComponent<AIScript> ();
                aiScript.TakeHit (damage);
            }
        } else {
            if (aiScript.isAttacking && other.gameObject.GetComponent<PlayerController> () != null) {
                playerController = other.gameObject.GetComponent<PlayerController> ();
                playerController.TakeHit (damage);
            }
        }
    }
}