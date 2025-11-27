using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerBase : BuildingBase
{
    [Header("References")]
    [SerializeField] protected Transform _front;
    [SerializeField] protected GameObject _ammo;

    [Header("Firing")]
    [SerializeField] protected float _fireAngle = 90f;
    [SerializeField] protected float _fireRange = 5f;
    [SerializeField] protected float _fireRate = 1f;
    [SerializeField] protected int _damage = 0;

    [Header("Layers")]
    [SerializeField] private LayerMask _enemyLayer;

    private float _fireCooldown;

    // Update is called once per frame
    void Update()
    {
        _fireCooldown -= Time.deltaTime;


        if (_fireCooldown <= 0f)
        {
            Transform target = GetTargetInRange();
            if(target != null){
                FireAt(target);
                _fireCooldown = 1f / _fireRate;
            }
        }
        
    }

    Transform GetTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _fireRange, _enemyLayer);

        float bestDist = float.MaxValue;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            //If enemy is dead, do not target
            if (hit.GetComponent<EnemyController>().isDead()) continue;

            Vector3 toTarget = hit.transform.position - transform.position;
            // Check angle
            if (Vector3.Angle(_front.forward, toTarget) > _fireAngle * 0.5f)
                continue;

            float dist = toTarget.sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }

    private void FireAt(Transform target)
    {
        Instantiate(_ammo, _front.position, _front.rotation).GetComponent<AmmoBase>().Initialize(target, _damage);
    }

    public override void OnSelect()
    {
        UIManager.instance.OpenUpgradeMenu(this);
    }

    public override void OnDeselect()
    {
        UIManager.instance.CloseMenu();
    }
}
