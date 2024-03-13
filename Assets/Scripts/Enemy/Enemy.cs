using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySO enemySO;
    public int level;
    private float health;
    public int id;
    public int nodeIndex;

    public void Init()
    {
        health = enemySO.enemyLevels[level].MaxHealth;
        transform.position = GameLoopManager.nodePositions[0];
        nodeIndex = 0;
    }
}
