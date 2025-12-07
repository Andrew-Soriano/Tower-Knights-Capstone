using System;
using UnityEngine;

public class CastleController : MonoBehaviour, IClickable, ISelectable
{
    public static CastleController instance;

    public Resources stockpile= new ();

    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;

    public static event Action gameOver;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
        if (stockpile == null)
            Debug.LogError("CastleController: stockpile is not assigned!");

        _HP = _maxHP;
    }

    private void Start()
    {
        UIManager.instance.StatusBarContoller.RefreshResourceUI(stockpile);
        UIManager.instance.StatusBarContoller.RefreshCastleHP(_HP, _maxHP);
    }

    private void OnEnable()
    {
        TowerTile.towerBuilt += PayForBuild;
        ResourceTile.resourceBuilt += PayForBuild;
    }

    private void OnDisable()
    {
        TowerTile.towerBuilt -= PayForBuild;
        ResourceTile.resourceBuilt -= PayForBuild;
    }

    public void OnClicked()
    {
        SelectionManager.instance.Select(this);
    }

    public void OnSelect()
    {
    }

    public void OnDeselect()
    {
    }

    public void trigger(EnemyController other)
    {
        _HP -= other.Damage;

        UIManager.instance.StatusBarContoller.RefreshCastleHP(_HP, _maxHP);
        UIManager.instance.StatusBarContoller.FlashCastleHP();

        if (_HP <= 0) gameOver?.Invoke();
    }

    public bool BuildTower(TowerTile tile, towerID id)
    {
        var towerData = TowerDatabase.Instance.GetTower(id);
        Resources cost = towerData.cost;
        if (stockpile.HasResources(cost))
        {
            tile.buildTower(id);
            return true;
        }
        
        return false;
    }

    public bool BuildResource(ResourceTile tile, towerID id)
    {
        var towerData = TowerDatabase.Instance.GetTower(id);
        Resources cost = towerData.cost;
        if (stockpile.HasResources(cost))
        {
            tile.buildResource(id);
            return true;
        }

        return false;
    }

    public bool AddResourceToStock(ResourceType type, int amount)
    {
        int current = type switch
        {
            ResourceType.Wood => stockpile.Wood,
            ResourceType.Planks => stockpile.Planks,
            ResourceType.Stone => stockpile.Stone,
            ResourceType.Bricks => stockpile.Bricks,
            ResourceType.Ore => stockpile.Ore,
            ResourceType.Metal => stockpile.Metal,
            ResourceType.Parts => stockpile.Parts,
            _ => 0
        };

        int clampedAmount = Mathf.Min(amount, 999 - current);

        if (clampedAmount > 0)
        {
            stockpile.Add(type, clampedAmount);
            UIManager.instance.StatusBarContoller.RefreshResourceUI(CastleController.instance.stockpile);
            return true;
        }

        return false;
    }

    public void SubtractFromStock(ResourceType type, int amount)
    {
        stockpile.Subtract(type, amount);
        UIManager.instance.StatusBarContoller.RefreshResourceUI(CastleController.instance.stockpile);
    }

    private void PayForBuild(Resources cost)
    {
        stockpile.Pay(cost);
        UIManager.instance.StatusBarContoller.RefreshResourceUI(CastleController.instance.stockpile);
    }
}
