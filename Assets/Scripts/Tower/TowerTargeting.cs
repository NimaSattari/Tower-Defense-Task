using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class TowerTargeting
{
    public enum TargetType
    {
        first,
        last,
        close
    }
    public static Enemy GetTarget(TowerBehaviour currentTower,TargetType targetMethod)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.range, currentTower.enemiesLayer);
        if(enemiesInRange.Length == 0)
        {
            return null;
        }
        NativeArray<EnemyData> enemiesToCalculate = new NativeArray<EnemyData>(enemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> nodePositions = new NativeArray<Vector3>(GameLoopManager.nodePositions, Allocator.TempJob);
        NativeArray<float> nodeDistances = new NativeArray<float>(GameLoopManager.nodeDistances, Allocator.TempJob);
        NativeArray<int> enemyToIndex = new NativeArray<int>(new int[] { -1 }, Allocator.TempJob);
        int enemyIndexToReturn = -1;

        for (int i = 0; i < enemiesToCalculate.Length; i++)
        {
            Enemy currentEnemy = enemiesInRange[i].GetComponent<Enemy>();
            int enemyIndexInList = EntityObjectPool.enemiesSpawned.FindIndex(x => x == currentEnemy);
            enemiesToCalculate[i] = new EnemyData(currentEnemy.transform.position, currentEnemy.nodeIndex, currentEnemy.health, enemyIndexInList);
        }

        SearchForEnemy searchForEnemy = new SearchForEnemy
        {
            enemiesToCalculate = enemiesToCalculate,
            nodePositions = nodePositions,
            nodeDistances = nodeDistances,
            enemyToIndex = enemyToIndex,
            compareValue = Mathf.Infinity,
            targetingType = (int)targetMethod,
            towerPosition = currentTower.transform.position
        };

        switch ((int)targetMethod)
        {
            case 0:
                searchForEnemy.compareValue = Mathf.Infinity;
                break;
            case 1:
                searchForEnemy.compareValue = Mathf.NegativeInfinity;
                break;
            case 2:
                goto case 0;
        }

        JobHandle dependency = new JobHandle();
        JobHandle searchHandle = searchForEnemy.Schedule(enemiesToCalculate.Length, dependency);
        searchHandle.Complete();

        if (enemyToIndex[0] != -1)
        {
            enemyIndexToReturn = enemiesToCalculate[enemyToIndex[0]].enemyIndex;

            enemiesToCalculate.Dispose();
            nodeDistances.Dispose();
            nodePositions.Dispose();
            enemyToIndex.Dispose();

            return EntityObjectPool.enemiesSpawned[enemyIndexToReturn];
        }

        enemiesToCalculate.Dispose();
        nodeDistances.Dispose();
        nodePositions.Dispose();
        enemyToIndex.Dispose();

        return null;
    }

    struct EnemyData
    {
        public EnemyData(Vector3 enemyPosition,int nodeIndex,float health, int enemyIndex)
        {
            this.enemyPosition = enemyPosition;
            this.nodeIndex = nodeIndex;
            this.enemyIndex = enemyIndex;
            this.health = health;
        }

        public Vector3 enemyPosition;
        public int enemyIndex;
        public int nodeIndex;
        public float health;
    }

    struct SearchForEnemy : IJobFor
    {
        [ReadOnly] public NativeArray<EnemyData> enemiesToCalculate;
        [ReadOnly] public NativeArray<Vector3> nodePositions;
        [ReadOnly] public NativeArray<float> nodeDistances;
        [NativeDisableParallelForRestriction] public NativeArray<int> enemyToIndex;

        public Vector3 towerPosition;
        public float compareValue;
        public int targetingType;

        public void Execute(int index)
        {
            float currentEnemyDistanceToEnd;
            float distanceToEnemy;
            switch (targetingType)
            {
                case 0:
                    currentEnemyDistanceToEnd = GetDistanceToEnd(enemiesToCalculate[index]);
                    if (currentEnemyDistanceToEnd < compareValue)
                    {
                        enemyToIndex[0] = index;
                        compareValue = currentEnemyDistanceToEnd;
                    }
                    break;
                case 1:
                    currentEnemyDistanceToEnd = GetDistanceToEnd(enemiesToCalculate[index]);
                    if (currentEnemyDistanceToEnd > compareValue)
                    {
                        enemyToIndex[0] = index;
                        compareValue = currentEnemyDistanceToEnd;
                    }
                    break;
                case 2:
                    distanceToEnemy = Vector3.Distance(towerPosition, enemiesToCalculate[index].enemyPosition);
                    if (distanceToEnemy > compareValue)
                    {
                        enemyToIndex[0] = index;
                        compareValue = distanceToEnemy;
                    }
                    break;
                default:
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData enemyToEvaluate)
        {
            float finalDistance = Vector3.Distance(enemyToEvaluate.enemyPosition, nodePositions[enemyToEvaluate.nodeIndex]);

            for (int i = enemyToEvaluate.nodeIndex; i < nodeDistances.Length; i++)
            {
                finalDistance += nodeDistances[i];
            }
            return finalDistance;
        }
    }
}
