using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private GameObject enemySmall;
    [SerializeField] private GameObject enemyMedium;
    [SerializeField] private GameObject enemyHeavy;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;
    [SerializeField] private Transform spawn3;

    [Header("Countdown")]
    [SerializeField] private float countdown;

    public int spawnCost;

    public List<Enemy> enemies = new List<Enemy>();
    public int currentWave;
    public int waveValue;

    void Start()
    {
        
    }

    void FixedUpdate()
    {

    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue>0)
        {
            //int randomEnemyID = Random.Range
        }
    }

    private void GenerateWave()
    {
        waveValue = currentWave;
        GenerateEnemies();
    }
}

