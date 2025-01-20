using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] public Transform spawn1;
    [SerializeField] public Transform spawn2;
    [SerializeField] public Transform spawn3;

    [Header("Countdown")]
    [SerializeField] public float countdown;
    public List<EnemyToSpawn> enemies = new List<EnemyToSpawn>();
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public int currentWave;
    public int waveValue;

    void Start()
    {
        GenerateEnemies();
    }

    void FixedUpdate()
    {

    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue>0)
        {
            int randomEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randomEnemyId].spawnCost;

            if(waveValue-randEnemyCost>=0)
            {
                generatedEnemies.Add(enemies[randomEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue<=0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    private void GenerateWave()
    {
        waveValue = currentWave;
        GenerateEnemies();
    }
}

[System.Serializable]
public class EnemyToSpawn
{
    public GameObject enemyPrefab;
    public int spawnCost;
}

