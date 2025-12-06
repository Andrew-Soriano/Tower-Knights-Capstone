using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TowerTile : BuildableTile
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject _ArcherTower;
    [SerializeField] private GameObject _CatapultTower;
    [SerializeField] private GameObject _BombTower;
    [SerializeField] private GameObject _AlchemistTower;

    [Header("Crafting Prefabs")]
    [SerializeField] private GameObject _SawmillTower;

    public void buildTower(towerID id)
    {
        GameObject tower;
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
            case towerID.Alchemist:
                tower = Instantiate(_AlchemistTower, transform.position, Quaternion.identity);
                break;
            case towerID.Sawmill:
                tower = Instantiate(_SawmillTower, transform.position, Quaternion.identity);
                break;
        }

        gameObject.SetActive(false);
    }

    public override void OnSelect()
    {
        UIManager.instance.BuildMenuControler.OpenBuildMenu();
        base.OnSelect();
    }

    public override void OnDeselect()
    {
        UIManager.instance.CloseMenu();
        base.OnSelect();
    }
}
