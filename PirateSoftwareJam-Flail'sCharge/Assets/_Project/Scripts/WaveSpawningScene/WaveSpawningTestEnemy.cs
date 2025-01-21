using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawningTestEnemy : MonoBehaviour
{
    [Header("Player Character")]
    [SerializeField] private Transform player;
    private WaveSpawner waveSpawner; // Reference to WaveSpawner

    private NavMeshAgent agent;
    void Start()
    {
        // Start the coroutine to delay initialization
        StartCoroutine(DelayStart());
        GameObject waveSpawnerObject = GameObject.FindFirstObjectByType<WaveSpawner>().gameObject;
        if (waveSpawnerObject != null)
            {
                waveSpawner = waveSpawnerObject.GetComponent<WaveSpawner>();
            }
        else
            {
                Debug.LogError("WaveSpawner object not found! Make sure there is a WaveSpawner script in the scene.");
            }
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(2.0f);
        // Automatically find the player transform
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found! Make sure it is tagged as 'Player'.");
        }

        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        if (player == null) return; // This code will force an exit if player is null instead of looping
        agent.destination = player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            waveSpawner.HandleEnemyDeath();
        }
    }
}
