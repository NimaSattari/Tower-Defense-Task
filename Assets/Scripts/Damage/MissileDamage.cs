using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDamage : MonoBehaviour, IDamage
{
    public LayerMask enemiesLayer;
    [SerializeField] private ParticleSystem missileParticle;
    [SerializeField] private Transform towerHead;

    private ParticleSystem.MainModule mainModule;
    [NonSerialized] public float damage;
    private float fireRate;
    private float delay;

    public void Init(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        delay = 1f / fireRate;
        mainModule = missileParticle.main;
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

            mainModule.startRotationX = towerHead.forward.x;
            mainModule.startRotationY = towerHead.forward.y;
            mainModule.startRotationZ = towerHead.forward.z;

            missileParticle.Play();

            delay = 1f / fireRate;
        }
    }
}
