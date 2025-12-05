using UnityEngine;

public class TowerCatapult : TowerBase
{
    [SerializeField] private GameObject _spikedAmmo;
    [SerializeField] private int _pierce;
    private GameObject _loadedBoulder;
    private bool _roundStarted = false;
    private Vector3 _target;
    private Animator _anim;
    private int _animShoot;

    protected override void Awake()
    {
        base.Awake();

        _damage = 10;
        _pierce = 10;

        _actions[0].action = UpgradeShootSpeed;
        _actions[1].action = UpgradeShotDamage;
        _actions[2].action = UpgradePierce;
        _actions[3].action = UpgradeSpiked;

        _anim = GetComponent<Animator>();
        _animShoot = Animator.StringToHash("Fire");
    }

    protected override void Update()
    {
        if (_roundStarted && _target != Vector3.zero)
        {
            var state = _anim.GetCurrentAnimatorStateInfo(0);
            if(state.IsName("Idle"))
            {
                _fireCooldown -= Time.deltaTime;


                if (_fireCooldown <= 0f)
                {
                    _anim.SetTrigger(_animShoot);
                }
            }
        }
    }
    private void OnEnable()
    {
        UIManager.nextRoundClicked += RoundStart;
        EnemyBaseController.endOfRound += RoundEnd;
    }
    private void OnDisable()
    {
        UIManager.nextRoundClicked -= RoundStart;
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

    public void Shoot()
    {
        if (_loadedBoulder == null) return;

        _loadedBoulder.transform.SetParent(null);
        
        _loadedBoulder.GetComponent<Boulder>().Initialize(_target, _damage, _pierce, _loadedBoulder.transform.position);

        _fireCooldown = 1f / _fireRate;
        _loadedBoulder = null;
    }

    public void Reload()
    {
        _loadedBoulder = Instantiate(_ammo, _front.transform.position, Quaternion.identity);
        _loadedBoulder.transform.SetParent(_front);
    }

    public void SetTarget(Vector3 pos)
    {
        if(Vector3.Distance(pos, transform.position) <= _fireRange)
            _target = pos;

        FlashRotateTurret(_target);
    }

    public void ShowTargetRange()
    {
        Vector3 towerPos = transform.position;

        // Find all tiles in the scene
        BuildableTile[] tiles = FindObjectsByType<BuildableTile>(FindObjectsSortMode.None);

        foreach (var tile in tiles)
        {
            float distance = Vector3.Distance(towerPos, tile.transform.position);

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
        BuildableTile[] tiles = FindObjectsByType<BuildableTile>(FindObjectsSortMode.None);
        foreach (var tile in tiles)
            tile.ResetHighlight();
    }

    private void UpgradeShootSpeed()
    {
        _fireRate += 1;
    }

    private void UpgradeShotDamage()
    {
        _damage += 10;
    }

    private void UpgradePierce()
    {
        _pierce += 5;
    }

    private void UpgradeSpiked()
    {
        _ammo = _spikedAmmo;
    }
}
