using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "WaveData", menuName = "Scriptable Objects/WaveData"), Serializable]
public class WaveData : ScriptableObject {
    [SerializeField] public int waveNumber;

    [SerializeField] public GameObject[] ListOfUnitsToSpawn;
    [SerializeField] public Transform[] ListOfUnitsSpawnLocations;
    [SerializeField] public int maxEnemiesAlive;
}