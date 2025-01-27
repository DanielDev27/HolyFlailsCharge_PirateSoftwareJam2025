using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveSpawner : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private float countdownTimer;

    [SerializeField] private int currentWaveNumber = 1;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private bool waveInProgress = false;
    [SerializeField] private Queue<GameObject[]> enemyQueue = new Queue<GameObject[]>(); // Queue to store the enemy prefabs to spawn

    [Header("Enemies")]
    [SerializeField] private GameObject[] enemyPrefabs; // This is my array of different enemy prefabs, using an array for scalability

    [SerializeField] private int maxEnemiesAlive = 15; // Maximum number of enemies alive at any given time

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints; // This is my array of different spawn points, also using an array for scalability

    [Header("Wave Settings")]
    [FormerlySerializedAs("countdown")]
    [SerializeField] private float waveCoolDown = 8f; // This is the time between waves

    [SerializeField] private float enemySpawnDelay = 1.5f;
    [SerializeField] List<WaveData> waveData = new List<WaveData>();
    [SerializeField] WaveData currentWaveData;
    [SerializeField] private int enemiesPerWave;

    [SerializeField] private float spawnVariance = 3f; // This will control the maximum distance between spawns of the spawn point. This way the enemy doesn't spawn on once another.
    [Header("Audio Manager Script")]
    [SerializeField] public AudioManager audioManagerScript;
    private bool IsWolfBossWave(int waveNumber) => waveNumber > 0 && waveNumber % 5 == 0 && waveNumber != 20;
    private bool IsMajorBossWave(int waveNumber) => waveNumber > 0 && waveNumber % 20 == 0;
    //********
    //Commented Out Variables
    //********

    //[SerializeField] private GameObject[] miniBossPrefab;
    //[SerializeField] private GameObject[] bossPrefab;
    //[SerializeField] private List<int> miniBossWaves = new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45 };
    //[SerializeField] private int bossWaveNumber = 20;
    //[SerializeField] private int testEnemiesToSpawn = 3;
    //[SerializeField] private float miniBossSpawnDelay = 5f;
    //[SerializeField] private float finalBossSpawnDelay = 10f;

    void Start()
    {
        countdownTimer = waveCoolDown;
        currentWaveNumber = 0;
    }

    void Update()
    {
        // Debugging, testing if spawning works the way I want it to
        // Remember to remove before final build NB!
        /*if (Input.GetKeyDown (KeyCode.M)) {
            Debug.Log ("M key pressed! Spawning test enemies...");
            SpawnTestEnemies (); // Testing to see what will happen if there are a LOT of enemies alive and if the queue works
        }

        if (Input.GetKeyDown (KeyCode.C)) // Debugging to see what will happen when all enemies are dead
        {
            CullEnemies ();
        }

        if (Input.GetKeyDown (KeyCode.N)) // Debugging to see what will happen when all enemies are dead
        {
            SkipWave ();
        }*/
    }

    void FixedUpdate()
    {
        if (!ScoreSystem.instance.isGameOver)
        {
            if (waveData[currentWaveNumber] == null)
            {
                ScoreSystem.instance.TriggerGameEnd();
                return;
            }
        }
        if (!waveInProgress && enemiesAlive == 0)
        // Checking if there is no wave in progress and if there are no enemies alive before I run the code
        {
            NextWave();
            ScoreSystem.instance.IncreaseWaveCounter();
        }
    }

    //Non-Test Functions
    private void NextWave()
    {
        countdownTimer -= Time.deltaTime;
        if (countdownTimer <= 0)
        {
            CullEnemies();
            Debug.Log("_________________________________");
            Debug.Log("____________Next Wave____________");
            Debug.Log("_________________________________");
            //enemiesPerWave = currentWaveNumber + 2;
            SetWaveInfo();
            GenerateWave();
            countdownTimer = waveCoolDown;
            currentWaveNumber = currentWaveData.waveNumber;
            HUD.instance?.IncrementWaveCount();
            PlaySpawnSound();
        }
    }

    private void PlaySpawnSound()
    {
        if (IsWolfBossWave(currentWaveNumber))
        {
            AudioManager.PlaySound((int)SoundType.SPAWNWOLFBOSS);
        }
        else if (IsMajorBossWave(currentWaveNumber))
        {
            AudioManager.PlaySound((int)SoundType.SPAWNBOSS);
        }
        else
        {
            AudioManager.PlaySound((int)SoundType.SPAWNWAVE);
        }
    }

    void SetWaveInfo()
    {
        //Information about the wave is inherited from the Wave Data SO
        currentWaveData = waveData[currentWaveNumber];
        enemiesPerWave = currentWaveData.ListOfUnitsToSpawn.Length;
        Debug.Log("The current wave number is: " + currentWaveNumber + ", Spawning " + enemiesPerWave + " enemies");
        enemyPrefabs = currentWaveData.ListOfUnitsToSpawn;
        spawnPoints = currentWaveData.ListOfUnitsSpawnLocations;
        maxEnemiesAlive = currentWaveData.maxEnemiesAlive;
        waveInProgress = true;
    }

    private void GenerateWave()
    {
        // Start the coroutine to spawn enemies with a delay
        StartCoroutine(SpawnEnemiesWithDelay());

        /*        if (miniBossWaves.Contains (currentWaveNumber)) {
                    Debug.Log ("One of the enemies is a MINI BOSS");
                    StartCoroutine (SpawnMiniBossWithDelay ());
                }

                if (currentWaveNumber == bossWaveNumber) {
                    StartCoroutine (SpawnBossWithDelay ());
                    Debug.Log ("One of the enemies is a BIG BAD BOSS");
                }*/
    }


    private IEnumerator SpawnEnemiesWithDelay()
    {
        for (int i = 0; i < enemiesPerWave; i++) // Spawn the specified number of enemies for the wave
        {
            SpawnEntity(enemyPrefabs);
            yield return new WaitForSeconds(enemySpawnDelay); // Wait for a specific amount of time before spawning the next enemy
        }
    }

    /*private IEnumerator SpawnMiniBossWithDelay () {
        yield return new WaitForSeconds (miniBossSpawnDelay); // ...before spawning the mini boss
        SpawnEntity (miniBossPrefab);
    }

    private IEnumerator SpawnBossWithDelay () {
        yield return new WaitForSeconds (finalBossSpawnDelay); // ...before spawning the Final boss
        SpawnEntity (bossPrefab);
    }
*/
    private void SpawnEntity(GameObject[] entityPrefabs)
    {
        if (!waveInProgress) return;

        if (enemiesAlive >= maxEnemiesAlive) // Here, if the max enemies alive limit is reached, it will queue the enemy instead of spawning it
        {
            enemyQueue.Enqueue(entityPrefabs);
            Debug.Log("Enemy queued for spawning. Total queued: " + enemyQueue.Count);
            return; // However, if we're below, it will spawn it right away
        }

        int randomEntityIndex = Random.Range(0, entityPrefabs.Length); // This randomly selects a prefab from the array
        GameObject randomEntity = entityPrefabs[randomEntityIndex];

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length); // This randomly selects a spawn point from the array
        Transform randomSpawnPoint = spawnPoints[randomSpawnPointIndex];

        Vector3 spawnPosition = randomSpawnPoint.position + new Vector3( // This randomly gives it some variance from the spawn point
            Random.Range(-spawnVariance, spawnVariance),
            0f,
            Random.Range(-spawnVariance, spawnVariance)
        );

        Instantiate(randomEntity, spawnPosition, randomSpawnPoint.rotation); // This then spawns the enemy at the position of the randomly selected spawn point
        enemiesAlive++;
        Debug.Log("There are " + enemiesAlive + " enemies alive"); // spawn point and assigns that enemy the variable "spawnedEnemy"
    }


    public void HandleEnemyDeath()
    {
        enemiesAlive = Mathf.Max(enemiesAlive - 1, 0); // Preventing a negative

        Debug.Log("There are " + enemiesAlive + " enemies alive");

        if (enemiesAlive == 0)
        {
            waveInProgress = false;
        }


        if (enemiesAlive < maxEnemiesAlive && enemyQueue.Count > 0) // Here I'm checking if an enemy needs to be spawned from the queue when an enemy dies
        {
            GameObject[] nextEnemy = enemyQueue.Dequeue(); // Here I am removing the enemy that is being spawned from the queue
            SpawnEntity(nextEnemy);
            Debug.Log("Spawning an enemy from the queue. Total queued: " + enemyQueue.Count);
        }
    }
    // Debugging, testing if spawning works the way I want it to
    // Remember to remove before final build NB!

    //Test Functions
    /*private void SpawnTestEnemies () {
        for (int i = 0; i <= testEnemiesToSpawn; i++) {
            SpawnEntity (enemyPrefabs);
        }
    }*/

    private void CullEnemies() // if everything is working correctly, this method can be removed. It's a failsafe.
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

    /*private void SkipWave()
    {
        currentWaveNumber++;
        waveInProgress = false;
        Debug.Log("Skipped to wave: " + currentWaveNumber);
        CullEnemies();
    }
    */
}