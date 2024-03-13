using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObjectPool : MonoBehaviour
{
    public static List<Enemy> enemiesSpawned;
    public static List<Transform> enemiesSpawnedTransform;
    public static Dictionary<int, GameObject> enemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> enemyObjectPools;

    private static bool IsInitialized;

    public static void Init()
    {
        if(!IsInitialized)
        {
            enemyPrefabs = new Dictionary<int, GameObject>();
            enemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            enemiesSpawned = new List<Enemy>();
            enemiesSpawnedTransform = new List<Transform>();

            EnemySpawnData[] enemySpawnDatas = Resources.LoadAll<EnemySpawnData>("EnemySpawnData");

            foreach (var enemy in enemySpawnDatas)
            {
                enemyPrefabs.Add(enemy.enemyID, enemy.enemyPrefab);
                enemyObjectPools.Add(enemy.enemyID, new Queue<Enemy>());
            }
            IsInitialized = true;
        }
    }

    public static Enemy EnemyPrefab(int enemyID)
    {
        Enemy enemyInstance = null;

        if(enemyPrefabs.ContainsKey(enemyID))
        {
            Queue<Enemy> refrenceQueue = enemyObjectPools[enemyID];
            if(refrenceQueue.Count > 0)
            {
                enemyInstance = refrenceQueue.Dequeue();
                enemyInstance.Init();
                enemyInstance.gameObject.SetActive(true);
            }
            else
            {
                GameObject NewEnemy = Instantiate(enemyPrefabs[enemyID], GameLoopManager.nodePositions[0], Quaternion.identity);
                enemyInstance = NewEnemy.GetComponent<Enemy>();
                enemyInstance.Init();
            }
        }
        else
        {
            return null;
        }
        enemiesSpawnedTransform.Add(enemyInstance.transform);
        enemiesSpawned.Add(enemyInstance);
        enemyInstance.id = enemyID;
        return enemyInstance;
    }

    public static void RemoveEnemy(Enemy enemyToRemove)
    {
        enemyObjectPools[enemyToRemove.id].Enqueue(enemyToRemove);
        enemyToRemove.gameObject.SetActive(false);
        enemiesSpawnedTransform.Remove(enemyToRemove.transform);
        enemiesSpawned.Remove(enemyToRemove);
    }
}
