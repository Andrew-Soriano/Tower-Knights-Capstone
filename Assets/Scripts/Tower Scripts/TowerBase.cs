using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerBase : BuildingBase
{
    [Header("References")]
    [SerializeField] protected Transform _front;
    [SerializeField] protected GameObject _ammo;
    [SerializeField] private Transform _rotatingPart;

    [Header("Firing")]
    [SerializeField] protected float _fireAngle = 90f;
    [SerializeField] protected float _fireRange = 5f;
    [SerializeField] protected float _fireRate = 1f;
    [SerializeField] protected int _damage = 10;

    [Header("Layers")]
    [SerializeField] protected LayerMask _enemyLayer;

    protected float _fireCooldown = 2f;

    // Update is called once per frame
    protected virtual void Update()
    {
        _fireCooldown -= Time.deltaTime;


        if (_fireCooldown <= 0f)
        {
            Transform target = GetTargetInRange();
            if(target != null){
                FlashRotateTurret(target.position);
                FireAt(target);
                _fireCooldown = 2f / _fireRate;
            }
        }
        
    }

    protected virtual Transform GetTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _fireRange, _enemyLayer);

        float maxProgress = float.MinValue;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            var enemy = hit.GetComponentInParent<EnemyController>();
            if (enemy.isDead()) continue;

            Vector3 toTarget = enemy.transform.position - transform.position;

            if (Vector3.Angle(_front.forward, toTarget) > _fireAngle * 0.5f)
                continue;

            float progress = enemy.Path_Progress;
            if (progress > maxProgress)
            {
                maxProgress = progress;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }

    protected virtual void FireAt(Transform target)
    {
        Instantiate(_ammo, _front.position, _front.rotation).GetComponent<AmmoBase>().Initialize(target, _damage);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        UIManager.instance.UpgradeMenuContoller.OpenUpgradeMenu(this);
    }

    public override void OnDeselect()
    {
        UIManager.instance.CloseMenu();
    }

    public void FlashRotateTurret(Vector3 target)
    {
        if (_rotatingPart != null)
        {
            Vector3 dir = target - _rotatingPart.position;
            dir.y = 0;
            _rotatingPart.rotation = Quaternion.LookRotation(dir);
        }
    }
}
