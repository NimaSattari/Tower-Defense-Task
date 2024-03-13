using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnData",menuName = "Enemy Spawn Data")]
public class EnemySpawnData : ScriptableObject
{
    public GameObject enemyPrefab;
    public int enemyID;
}
