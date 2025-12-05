using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum towerID
{
    Archer,
    Catapult,
    Bomb,
    Sawmill
}

public class TowerTile : BuildableTile
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject _ArcherTower;
    [SerializeField] private GameObject _CatapultTower;
    [SerializeField] private GameObject _BombTower;

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
            case towerID.Catapult:
                tower = Instantiate(_CatapultTower, transform.position, Quaternion.identity);
                break;
            case towerID.Bomb:
                tower = Instantiate(_BombTower, transform.position, Quaternion.identity);
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
