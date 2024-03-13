using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDamage))]
public class TowerBehaviour : MonoBehaviour
{
    public LayerMask enemiesLayer;

    public Enemy target;
    public Transform towerPivot;
    public float damage, fireRate, range;

    private float delay;

    private IDamage currentDamage;

    void Start()
    {
        currentDamage = GetComponent<IDamage>();

        delay = 1 / fireRate;

        currentDamage.Init(damage, fireRate);
    }

    public void Tick()
    {
        currentDamage.DamageTick(target);
        if(target != null)
        {
            towerPivot.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if(target != null)
        {
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.DrawLine(towerPivot.position, target.transform.position);
        }
    }
}
