using System.Collections.Generic;
using UnityEngine;

public class TowerBomb : TowerBase
{
    [SerializeField] private ArcRange _rangeArc;
    [SerializeField] private GameObject _stunBomb;
    [SerializeField] private float _stun;
    [SerializeField] private float _blastRadius;

    [SerializeField] private Animator _anim;
    private int _anim_fire;

    protected override void Awake()
    {
        base.Awake();

        _actions[0].action = UpgradeShootSpeed;
        _actions[1].action = UpgradeShotDamage;
        _actions[2].action = UpgradeBlastRadius;
        _actions[3].action = UpgradeStun;

        Animator anim = GetComponent<Animator>();
        _anim_fire = Animator.StringToHash("Fire");
    }
    public override void OnSelect()
    {

        if (_rangeArc != null)
        {
            _rangeArc.gameObject.SetActive(true);

            // Update arc to match our firing parameters
            _rangeArc.SetArc(_fireRange, _fireAngle);

            // Rotate the arc to match the tower's forward direction
            _rangeArc.transform.rotation = Quaternion.LookRotation(_front.forward, Vector3.up);

            //Fade it in
            _rangeArc.FadeIn();
        }

        base.OnSelect();
    }

    public override void OnDeselect()
    {
        if (_rangeArc != null)
        {
            _rangeArc.gameObject.SetActive(false);
        }

        base.OnDeselect();
    }

    protected override Transform GetTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _fireRange, _enemyLayer);
        if (hits.Length == 0) return null;

        Transform bestTarget = null;
        int maxEnemiesInRadius = 0;

        // Collect all valid enemies
        List<EnemyController> enemies = new();
        foreach (Collider hit in hits)
        {
            var enemy = hit.GetComponentInParent<EnemyController>();
            if (enemy != null && !enemy.isDead())
                enemies.Add(enemy);
        }

        // Count how many enemies are within blast radius of each enemy
        foreach (var enemy in enemies)
        {
            int count = 0;
            Vector3 pos = enemy.transform.position;
            foreach (var other in enemies)
            {
                if (Vector3.Distance(pos, other.transform.position) <= _blastRadius)
                    count++;
            }

            if (count > maxEnemiesInRadius)
            {
                maxEnemiesInRadius = count;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget;
    }

    protected override void FireAt(Transform target)
    {
        _anim.SetTrigger(_anim_fire);
        Instantiate(_ammo,
                    _front.position,
                    _front.rotation).GetComponent<bombTimer>().Initialize(target, _damage, _blastRadius, _stun);
    }

    private void UpgradeShootSpeed()
    {
        _fireRate += 1;
    }

    private void UpgradeShotDamage()
    {
        _damage += 5;
    }

    private void UpgradeBlastRadius()
    {
        _blastRadius += 2;
    }

    private void UpgradeStun()
    {
        _ammo = _stunBomb;
    }
}
