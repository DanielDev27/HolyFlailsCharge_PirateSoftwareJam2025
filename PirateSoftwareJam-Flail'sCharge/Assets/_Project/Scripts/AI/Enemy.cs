using System;
using UnityEngine;

[CreateAssetMenu (fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject {
    public EnemyType EnemyType;
    public GameObject EnemyModel;
    public float EnemyMoveSpeed;
    public float EnemyMaxHealth;
    public float EnemyDamage;
}

public enum EnemyType {
    Goblin,
    Orc,
    Wolf,
}