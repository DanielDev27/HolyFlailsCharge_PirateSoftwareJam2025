using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public int waveNumber;

    public List<GameObject> ListOfUnitsToSpawn = new List<GameObject>();
    public List<Transform> ListOfUnitsSpawnLocations = new List<Transform>();

}
