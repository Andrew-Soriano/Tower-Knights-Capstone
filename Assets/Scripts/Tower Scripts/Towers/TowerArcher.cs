using UnityEngine;

public class TowerArcher : TowerBase
{
    [SerializeField] private ArcRange _rangeArc;
    [SerializeField] private GameObject _fireArrow;

    protected override void Awake()
    {
        base.Awake();

        _damage = 10;

        _actions[0].action = UpgradeShootSpeed;
        _actions[1].action = UpgradeShotDamage;
        _actions[2].action = UpgradeSlowEnemy;
        _actions[3].action = UpgradeFireArrows;
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

    }

    private void UpgradeFireArrows()
    {
        _ammo = _fireArrow;
    }
}
