using System;
using UnityEngine;

public class CastleController : MonoBehaviour, IClickable, ISelectable
{
    public static CastleController instance;

    public Resources stockpile = new Resources();

    private void Awake()
    {
        //Singleton pattern
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIManager.instance.RefreshResourceUI(stockpile);
    }

    public void OnClicked()
    {
        SelectionManager.instance.Select(this);
    }

    public void OnSelect()
    {
        UIManager.instance.OpenCastleMenu();
    }

    public void OnDeselect()
    {
        UIManager.instance.CloseMenu();
    }

    internal void trigger(int id)
    {
    }

    public bool BuildTower(TowerTile tile, towerID id)
    {
        Resources cost = TowerData.GetCosts(id);

        if (stockpile.HasResources(cost))
        {
            stockpile.Pay(cost);

            tile.buildTower(id);
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
            UIManager.instance.RefreshResourceUI(CastleController.instance.stockpile);
            return true;
        }

        return false;
    }

    public void SubtractFromStock(ResourceType type, int amount)
    {
        stockpile.Subtract(type, amount);
        UIManager.instance.RefreshResourceUI(CastleController.instance.stockpile);
    }
}
