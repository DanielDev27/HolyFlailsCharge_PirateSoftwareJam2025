using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] bool Player;
    [SerializeField] PlayerController playerController;
    [SerializeField] AIScript aiScript;

    void OnTriggerEnter (Collider other) {
        if (Player) {
            playerController = this.GetComponentInParent<PlayerController> ();
            if (playerController.isAttacking && other.gameObject.GetComponent<AIScript> () != null) {
                Debug.Log ("Hit Enemy");
            }
        } else {
            aiScript = this.GetComponentInParent<AIScript> ();
            if (aiScript.isAttacking && other.gameObject.GetComponent<PlayerController> () != null) {
                Debug.Log ("Hit Player");
            }
        }
    }
}