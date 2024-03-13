using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    public static Vector3[] nodePositions;
    private static Queue<Enemy> enemiesToRemove;
    private static Queue<int> enemyIDsToPool;

    public Transform nodeParent;
    public bool loopShouldEnd;

    void Start()
    {
        enemyIDsToPool = new Queue<int>();
        enemiesToRemove = new Queue<Enemy>();
        EntityObjectPool.Init();
        nodePositions = new Vector3[nodeParent.childCount];
        for (int i = 0; i < nodePositions.Length; i++)
        {
            nodePositions[i] = nodeParent.GetChild(i).position;
        }

        StartCoroutine(GameLoop());
        InvokeRepeating("Test", 0, 1);
    }

    public void Test()
    {
        EnqueueEnemyIDToPool(1);
    }

    IEnumerator GameLoop()
    {
        while (!loopShouldEnd)
        {
            AddEnemies();

            NativeArray<Vector3> nodeToUse = new NativeArray<Vector3>(nodePositions, Allocator.TempJob);
            NativeArray<int> nodeIndices = new NativeArray<int>(EntityObjectPool.enemiesSpawned.Count, Allocator.TempJob);
            NativeArray<float> enemySpeeds = new NativeArray<float>(EntityObjectPool.enemiesSpawned.Count, Allocator.TempJob);
            TransformAccessArray enemyAccess = new TransformAccessArray(EntityObjectPool.enemiesSpawnedTransform.ToArray(), 2);
            for (int i = 0; i < EntityObjectPool.enemiesSpawned.Count; i++)
            {
                enemySpeeds[i] = EntityObjectPool.enemiesSpawned[i].enemySO.enemyLevels[EntityObjectPool.enemiesSpawned[i].level].Speed;
                nodeIndices[i] = EntityObjectPool.enemiesSpawned[i].nodeIndex;
            }

            MoveEnemiesJob moveEnemies = new MoveEnemiesJob
            {
                nodePositions = nodeToUse,
                enemySpeed = enemySpeeds,
                nodeIndex = nodeIndices,
                deltaTime = Time.deltaTime
            };

            JobHandle moveJobHandle = moveEnemies.Schedule(enemyAccess);
            moveJobHandle.Complete();

            for (int i = 0; i < EntityObjectPool.enemiesSpawned.Count; i++)
            {
                EntityObjectPool.enemiesSpawned[i].nodeIndex = nodeIndices[i];
                if(EntityObjectPool.enemiesSpawned[i].nodeIndex == nodePositions.Length)
                {
                    EnqueueEnemyToRemove(EntityObjectPool.enemiesSpawned[i]);
                }
            }

            enemySpeeds.Dispose();
            nodeIndices.Dispose();
            enemyAccess.Dispose();
            nodeToUse.Dispose();

            RemoveEnemies();

            yield return null;
        }
    }

    private static void RemoveEnemies()
    {
        if (enemiesToRemove.Count > 0)
        {
            for (int i = 0; i < enemiesToRemove.Count; i++)
            {
                EntityObjectPool.RemoveEnemy(enemiesToRemove.Dequeue());
            }
        }
    }

    private static void AddEnemies()
    {
        if (enemyIDsToPool.Count > 0)
        {
            for (int i = 0; i < enemyIDsToPool.Count; i++)
            {
                EntityObjectPool.EnemyPrefab(enemyIDsToPool.Dequeue());
            }
        }
    }

    public static void EnqueueEnemyIDToPool(int enemyID)
    {
        enemyIDsToPool.Enqueue(enemyID);
    }

    public static void EnqueueEnemyToRemove(Enemy enemyToRemove)
    {
        enemiesToRemove.Enqueue(enemyToRemove);
    }
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> nodeIndex;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> enemySpeed;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> nodePositions;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if (nodeIndex[index]<nodePositions.Length)
        {
            Vector3 positionToMoveTo = nodePositions[nodeIndex[index]];
            transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, enemySpeed[index] * deltaTime);

            if (transform.position == positionToMoveTo)
            {
                nodeIndex[index]++;
            }
        }
    }
}