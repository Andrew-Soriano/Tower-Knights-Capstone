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
        Resources cost = new Resources();
        switch (id)
        {
            case towerID.Archer:
                cost = TowerData.ArcherCost; break;
        }

        if (stockpile.HasResources(cost))
        {
            stockpile.Pay(cost);

            tile.buildTower(id);
            return true;
        }
        
        return false;
    }
}
