using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public void Init(float damage, float fireRate);
    public void DamageTick(Enemy target);
}

public class StandardDamage : MonoBehaviour, IDamage
{
    private float damage;
    private float fireRate;
    private float delay;

    public void Init(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        delay = 1f / fireRate;
    }

    public void DamageTick(Enemy target)
    {
        if (target)
        {
            if (delay > 0f)
            {
                delay -= Time.deltaTime;
                return;
            }

            GameLoopManager.EnqueueDamageData(new EnemyDamageData(target, damage, target.damageResistance));

            delay = 1f / fireRate;
        }
    }
}
