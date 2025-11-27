using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum towerID
{
    Archer,
    Sawmill
}

public class TowerTile : BuildableTile
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject _ArcherTower;

    [Header("Crafting Prefabs")]
    [SerializeField] private GameObject _SawmillTower;

    public void buildTower(towerID id)
    {
        GameObject tower;
        //Instantiate tower by id
        switch (id)
        {
            case towerID.Archer:
                tower = Instantiate(_ArcherTower, transform.position, Quaternion.identity);
                break;
            case towerID.Sawmill:
                tower = Instantiate(_SawmillTower, transform.position, Quaternion.identity);
                break;
        }

        //Remove this tile (tower has replaced it)
        gameObject.SetActive(false);
    }

    public override void OnSelect()
    {
        UIManager.instance.OpenBuildMenu();
        base.OnSelect();
    }

    public override void OnDeselect()
    {
        UIManager.instance.CloseMenu();
        base.OnSelect();
    }
}
