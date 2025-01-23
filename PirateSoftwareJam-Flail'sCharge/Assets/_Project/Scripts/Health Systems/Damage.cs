using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] bool Player;
    [SerializeField] PlayerController playerController;
    [SerializeField] AIScript aiScript;

    [SerializeField] int damage;
    //Damage trigger for entering a collider

    void Awake()
    {
        //Fill hp to its max
        if (Player)
        {
            playerController = this.GetComponentInParent<PlayerController>();
            Player = true;
        }
        else
        {
            aiScript = this.GetComponentInParent<AIScript>();

        }

        //OnCooldown = false;
        //_coroutineActivated = false;

    }
    void OnTriggerEnter(Collider other)
    {
        if (Player)
        {
            playerController = this.GetComponentInParent<PlayerController>();
            if (playerController.isAttacking && other.gameObject.GetComponent<AIScript>() != null)
            {
                //Debug.Log ("Hit Enemy");
                aiScript = other.GetComponentInParent<AIScript>();
                aiScript.TakeHit(damage);
            }
        }
        else
        {
            if (aiScript.isAttacking && other.gameObject.GetComponent<PlayerController>() != null)
            {
                playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.TakeHit(damage);
            }
        }
    }
}
