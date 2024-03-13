using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCollision : MonoBehaviour
{
    [SerializeField] private MissileDamage baseClass;
    [SerializeField] private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> particleCollisionEvents;
    private float range;

    private void Start()
    {
        range = baseClass.GetComponent<TowerBehaviour>().range;
        particleCollisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        _particleSystem.GetCollisionEvents(other, particleCollisionEvents);

        for (int i = 0; i < particleCollisionEvents.Count; i++)
        {
            Collider[] ememiesInRadius = Physics.OverlapSphere(particleCollisionEvents[i].intersection, range, baseClass.enemiesLayer);

            for (int j = 0; j < ememiesInRadius.Length; j++)
            {
                Enemy enemy = EntityObjectPool.enemyTransformPairs[ememiesInRadius[i].transform];
                EnemyDamageData damageToApply = new EnemyDamageData(enemy, baseClass.damage, enemy.damageResistance);
                GameLoopManager.EnqueueDamageData(damageToApply);
            }
        }
    }
}
