using Neu.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDamage))]
public class TowerBehaviour : MonoBehaviour
{
    [SerializeField] private DoTweenActions placementAnim;
    public LayerMask enemiesLayer;

    public Enemy target;
    public Transform towerPivot;
    public float damage, fireRate, range;
    public int cost;

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
            towerPivot.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position - new Vector3(0, +1.5f, 0));
        }
    }

    public void PlacementAnim()
    {
        placementAnim.DoAnimation();
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
