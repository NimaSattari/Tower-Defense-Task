using Neu.Animations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] DoTweenActions dieAnim;

    public EnemySO enemySO;
    public int level;
    [NonSerialized] public float health;
    [NonSerialized] public int reward;
    [NonSerialized] public float damageResistance;
    public int id;
    public int nodeIndex;

    public void Init()
    {
        health = enemySO.enemyLevels[level].MaxHealth;
        damageResistance = enemySO.enemyLevels[level].DamageResistance;
        reward = enemySO.enemyLevels[level].reward;
        transform.position = GameLoopManager.nodePositions[0];
        nodeIndex = 0;
    }

    public void Die()
    {
        StartCoroutine(DieEnumerator());
    }

    private IEnumerator DieEnumerator()
    {
        dieAnim.DoAnimation();
        yield return dieAnim.animationDuration;
    }
}
