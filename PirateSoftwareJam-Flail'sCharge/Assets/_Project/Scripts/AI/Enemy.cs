using System;
using UnityEngine;

[CreateAssetMenu (fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject {
    public EnemyType EnemyType;
    public float EnemyMoveSpeed;
    public float EnemyMaxHealth;
    public float EnemyDamage;
    public float EnemyAttackCD;
}

public enum EnemyType {
    Goblin,
    Orc,
    Wolf,
    Mage,
}