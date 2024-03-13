using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviour, IDamage
{
    [SerializeField] private Transform laserPivot;
    [SerializeField] private LineRenderer lineRenderer;

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
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0,laserPivot.position);
            lineRenderer.SetPosition(1, target.transform.position);
            if (delay > 0f)
            {
                delay -= Time.deltaTime;
                return;
            }

            GameLoopManager.EnqueueDamageData(new EnemyDamageData(target, damage, target.damageResistance));

            delay = 1f / fireRate;
            return;
        }
        lineRenderer.enabled = false;
    }
}
