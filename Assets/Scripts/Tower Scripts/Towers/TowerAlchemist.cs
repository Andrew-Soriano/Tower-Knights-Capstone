using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TowerAlchemist : TowerBase
{
    [SerializeField] private GameObject _fireAmmo;
    [SerializeField] float _slow;
    [SerializeField] float _persist;
    private bool _roundStarted = false;
    private Vector3 _target;
    private UnBuildableTile _targetTile = null;
    private float _vulnerability;

    protected override void Awake()
    {
        base.Awake();

        _actions[0].action = UpgradeEffectUp;
        _actions[1].action = UpgradePermanance;
        _actions[2].action = UpgradeVulnerability;
        _actions[3].action = UpgradeFrostToFire;
    }

    protected override void Update()
    {
        if (_roundStarted && _target != Vector3.zero)
        {
            _fireCooldown -= Time.deltaTime;

            if (_fireCooldown <= 0f)
            {
                FireAt(_target);
                _fireCooldown = 2f / _fireRate;
            }
        }
    }
    private void OnEnable()
    {
        StatusBarController.nextRoundClicked += RoundStart;
        EnemyBaseController.endOfRound += RoundEnd;
    }
    private void OnDisable()
    {
        StatusBarController.nextRoundClicked -= RoundStart;
        EnemyBaseController.endOfRound -= RoundEnd;
    }

    public override void OnSelect()
    {
        base.OnSelect();
        ShowTargetRange();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        ClearTargetRange();
    }

    private void RoundStart() => _roundStarted = true;
    private void RoundEnd()=> _roundStarted = false;

    protected void FireAt(Vector3 target)
    {
        Instantiate(_ammo, _front.position, _front.rotation)
            .GetComponent<TowerFlask>()
            .Initialize(target, _damage, _slow, _persist, _vulnerability, _front.position, _targetTile);
    }

    public void SetTarget(UnBuildableTile tile)
    {
        float distance = Vector3.Distance(this.transform.position, tile.transform.position);

        if (distance <= _fireRange)
        {
            _target = tile.transform.position;
            _targetTile = tile;
        }
    }

    public void ShowTargetRange()
    {
        Vector3 towerPos = transform.position;

        UnBuildableTile[] tiles = FindObjectsByType<UnBuildableTile>(FindObjectsSortMode.None);
        Vector3 horizontalTowerPos = new Vector3(transform.position.x, 0, transform.position.z);

        foreach (var tile in tiles)
        {
            if (tile.GetType() != typeof(UnBuildableTile))
                continue;

            Vector3 horizontalTilePos = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
            float distance = Vector3.Distance(horizontalTowerPos, horizontalTilePos);

            if (tile.transform.position == _target)
                tile.Highlight(Color.blue);
            else if (distance <= _fireRange)
                tile.Highlight(Color.green);
            else
                tile.Highlight(Color.red);
        }
    }

    public void ClearTargetRange()
    {
        UnBuildableTile[] tiles = FindObjectsByType<UnBuildableTile>(FindObjectsSortMode.None);
        foreach (var tile in tiles)
        {
            if (tile.GetType() != typeof(UnBuildableTile))
                continue;
            tile.ResetHighlight();
        }
            
    }

    private void UpgradeEffectUp()
    {
        Debug.Log("Click");
        if(_slow > 0) _slow += 0.1f;
        else { _damage += 1; }
    }

    private void UpgradePermanance()
    {
        _persist += 5;
    }

    private void UpgradeVulnerability()
    {
        _vulnerability += .2f;
    }

    private void UpgradeFrostToFire()
    {
        _ammo = _fireAmmo;
        _damage += (int)(_slow * 10);
        _slow = 0;
        Debug.Log($"{name} upgraded to FrostToFire. Ammo: {_ammo.name}, Slow: {_slow}");
    }

    protected override Action GetUpgradeAction(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.EffectUp => UpgradeEffectUp,
            UpgradeType.Permanence => UpgradePermanance,
            UpgradeType.Vulnerability => UpgradeVulnerability,
            UpgradeType.FrostToFire => UpgradeFrostToFire,
            _ => base.GetUpgradeAction(type)
        };
    }
}
