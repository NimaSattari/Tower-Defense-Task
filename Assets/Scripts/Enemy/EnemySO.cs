using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy",menuName = "Enemy SO")]
public class EnemySO : ScriptableObject
{
    public List<EnemyLevel> enemyLevels = new List<EnemyLevel>();
}

[System.Serializable]
public class EnemyLevel
{
    public int level;
    public float MaxHealth;
    public float Speed;
    public float DamageResistance;
    public int reward;
}
