using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject {
    public int waveNumber;

    public GameObject[] ListOfUnitsToSpawn;
    public Transform[] ListOfUnitsSpawnLocations;
    public int maxEnemiesAlive;
    
}