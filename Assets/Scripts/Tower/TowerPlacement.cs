using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask placementCheckMask;
    [SerializeField] private LayerMask placementColliderMask;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerStats playerStats;

    private GameObject currentPlacingTower;

    void Update()
    {
        if(currentPlacingTower != null)
        {
            Ray ray =playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, 100f, placementColliderMask))
            {
                currentPlacingTower.transform.position = hitInfo.point;
            }
            if(Input.GetMouseButtonDown(1))
            {
                Destroy(currentPlacingTower);
                currentPlacingTower = null;
                return;
            }
            if(Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject != null)
            {
                if (!hitInfo.collider.gameObject.CompareTag("NoTower"))
                {
                    BoxCollider towerCollider = currentPlacingTower.gameObject.GetComponent<BoxCollider>();
                    towerCollider.isTrigger = true;
                    Vector3 boxCenter = currentPlacingTower.gameObject.transform.position + towerCollider.center;
                    Vector3 halfExtents = towerCollider.size / 2;
                    if (Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask, QueryTriggerInteraction.Ignore))
                    {
                        TowerBehaviour currentTowerBehaviour = currentPlacingTower.GetComponent<TowerBehaviour>();
                        GameLoopManager.towersInGame.Add(currentTowerBehaviour);
                        playerStats.AddMoney(-currentTowerBehaviour.cost);
                        towerCollider.isTrigger = false;
                        currentPlacingTower = null;
                    }
                }
            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        int towerCost = tower.GetComponent<TowerBehaviour>().cost;

        if(playerStats.GetMoney()>= towerCost)
        {
            currentPlacingTower = Instantiate(tower);
        }
    }
}
