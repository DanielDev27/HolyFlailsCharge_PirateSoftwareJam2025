using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] public GameObject[] enemyPrefabs; // This is my array of different enemy prefabs, using an array for scalability

    [Header("Spawn Points")]
    [SerializeField] public Transform[] spawnPoints; // This is my array of different spawn points, also using an array for scalability

    [Header("Wave Settings")]
    [SerializeField] public float countdown = 8f; // This is the time between waves
    [SerializeField] public int enemiesPerWave; 
    [SerializeField] public int testEnemiesToSpawn = 3;
    [SerializeField] public float spawnVariance = 3f; // This will control the maximum distance between spawns of the spawn point. This way the enemy doesn't spawn on once another.

    private float countdownTimer;
    private int currentWave = 1;
    private int enemiesAlive = 0; 
    private bool waveInProgress = false; 

    void Start()
    {
        // Initialize the countdown timer
        countdownTimer = countdown;

        enemiesPerWave = currentWave + 2;
    }
    

    void FixedUpdate()
    {
        // Checking if there is no wave in progress and if there are no enemies alive before I run the code
        if (!waveInProgress && enemiesAlive == 0)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                GenerateWave();
                waveInProgress = true; 
                countdownTimer = countdown; 
            }
        }
    }

    private void GenerateWave()
    {
        // Spawn the specified number of enemies for the wave
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        // This randomly selects a prefab from the array
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject randomEnemy = enemyPrefabs[randomEnemyIndex];

        // This randomly selects a spawn point from the array
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        Transform randomSpawnPoint = spawnPoints[randomSpawnPointIndex];

        Vector3 spawnPosition = randomSpawnPoint.position + new Vector3(
            Random.Range(-spawnVariance, spawnVariance),
            0f,
            Random.Range(-spawnVariance, spawnVariance)
        );

        // This then spawns the enemy at the position of the randomly selected spawn point and assigns that enemy the variable "spawnedEnemy"
        GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPosition, randomSpawnPoint.rotation);
        enemiesAlive++;

        // This code will detect when an enemy dies, which will then call on the HandleEnemyDeath method
        // spawnedEnemy.GetComponent<Enemy>().OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        enemiesAlive--;

        if (enemiesAlive == 0)
        {
            waveInProgress = false;
        }
    }
    // Debugging, testing if spawning works the way I want it to
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("M key pressed! Spawning test enemies...");
            SpawnTestEnemies(); // Call the test spawn function
        }
    }
    private void SpawnTestEnemies()
    {
        for (int i = 0; i < testEnemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }


}


