using System;
using UnityEngine;

public class TowerArcher : TowerBase
{
    [SerializeField] private ArcRange _rangeArc;
    [SerializeField] private GameObject _fireArrow;
    [SerializeField] private float _slow;

    protected override void Awake()
    {
        base.Awake();

        _actions[0].action = UpgradeShootSpeed;
        _actions[1].action = UpgradeShotDamage;
        _actions[2].action = UpgradeSlowEnemy;
        _actions[3].action = UpgradeFireArrows;
    }

    protected override void Update()
    {
        _fireCooldown -= Time.deltaTime;


        if (_fireCooldown <= 0f)
        {
            Transform target = GetTargetInRange();
            if (target != null)
            {
                FlashRotateTurret(target.position);
                SoundManager.instance.PlaySound(Sounds.ArrowLaunch);
                FireAt(target);
                _fireCooldown = 2f / _fireRate;
            }
        }
    }

    protected override void FireAt(Transform target)
    {
        Instantiate(_ammo).GetComponent<TowerArrow>().Initialize(target, _damage, _slow);
    }

    public override void OnSelect()
    {

        if (_rangeArc != null)
        {
            _rangeArc.gameObject.SetActive(true);

            _rangeArc.SetArc(_fireRange, _fireAngle);

            _rangeArc.transform.rotation = Quaternion.LookRotation(_front.forward, Vector3.up);

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

    private void UpgradeShootSpeed()
    {
        _fireRate += 1;
    }

    private void UpgradeShotDamage()
    {
        _damage += 10;
    }

    private void UpgradeSlowEnemy()
    {
        _slow += .2f;
    }

    private void UpgradeFireArrows()
    {
        _ammo = _fireArrow;
    }

    protected override Action GetUpgradeAction(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.ShotDamage => UpgradeShotDamage,
            UpgradeType.ShootSpeed => UpgradeShootSpeed,
            UpgradeType.SlowEnemy => UpgradeSlowEnemy,
            UpgradeType.FireArrows => UpgradeFireArrows,
            _ => base.GetUpgradeAction(type)
        };
    }
}
