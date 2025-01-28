using UnityEditor;

using UnityEngine;

#if UNITY_EDITOR
public class DeveloperMenu
{
    [MenuItem("DeveloperMenu / Kill Enemies")]
    public static void CullAllEnemies()
    {
        WaveSpawner.instance.CullEnemies();

        Debug.Log("Enemies Killed");
    }

}
#endif
