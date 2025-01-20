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
    [SerializeField] public float enemySpawnDelay = 1.5f;
    [SerializeField] public float spawnVariance = 3f; // This will control the maximum distance between spawns of the spawn point. This way the enemy doesn't spawn on once another.

    private float countdownTimer;
    private int currentWave = 1;
    private int enemiesAlive = 0; 
    private bool waveInProgress = false; 

    void Start()
    {
        countdownTimer = countdown;
    }
    

    void FixedUpdate()
    { 
        if (!waveInProgress && enemiesAlive == 0)       // Checking if there is no wave in progress and if there are no enemies alive before I run the code
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0)
        {
            Debug.Log("The current wave number is: " + currentWave);
            enemiesPerWave = currentWave + 2;
            Debug.Log("Spawning " + enemiesPerWave + " enemies");
            GenerateWave();
            waveInProgress = true;
            countdownTimer = countdown;
            currentWave++;
        }
    }

    private void GenerateWave()
    {
        StartCoroutine(SpawnEnemiesWithDelay());     // Start the coroutine to spawn enemies with a delay
    }

    private IEnumerator SpawnEnemiesWithDelay()
    {
        for (int i = 0; i < enemiesPerWave; i++)          // Spawn the specified number of enemies for the wave
        {
            SpawnEnemy();
            yield return new WaitForSeconds(enemySpawnDelay);   // Wait for a specific amount of time before spawning the next enemy
        }
    }

    private void SpawnEnemy()
    {
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);          // This randomly selects a prefab from the array
        GameObject randomEnemy = enemyPrefabs[randomEnemyIndex];

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);      // This randomly selects a spawn point from the array
        Transform randomSpawnPoint = spawnPoints[randomSpawnPointIndex];

        Vector3 spawnPosition = randomSpawnPoint.position + new Vector3(
            Random.Range(-spawnVariance, spawnVariance),
            0f,
            Random.Range(-spawnVariance, spawnVariance)
        );

        GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPosition, randomSpawnPoint.rotation); // This then spawns the enemy at the position of the randomly selected 
        enemiesAlive++;
        Debug.Log("There are " + enemiesAlive + " enemies alive");                                                                               // spawn point and assigns that enemy the variable "spawnedEnemy"

        // need to reference an enemy death here that will call on the method HandleEnemyDeath()
    }

    private void HandleEnemyDeath()
    {
        enemiesAlive--;

        Debug.Log("There are " + enemiesAlive + " enemies alive");

        if(enemiesAlive< 0) enemiesAlive = 0; // just trying to make sure it never goes into the negative for whatever reason

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
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("N key pressed! Moving to next wave...");
            enemiesAlive = 1; //forces HandleEnemyDeath() to succeed
            HandleEnemyDeath();
        }
        if (Input.GetKeyDown(KeyCode.C)) // Debugging to see what will happen when all enemies are dead
        {
            CullEnemies();
        }
    }
    private void SpawnTestEnemies()
    {
        for (int i = 0; i < testEnemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }
    private void CullEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Find all game objects with the tag "Enemy"

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy); // Destroy each enemy
        }

        enemiesAlive = 0; // Reset the enemiesAlive counter
        waveInProgress = false; // Reset the wave progress flag
        Debug.Log("All enemies have been culled.");
    }
}


