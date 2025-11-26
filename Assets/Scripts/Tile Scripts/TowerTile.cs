using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum towerID
{
    Archer
}

public class TowerTile : BuildableTile
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject _ArcherTower;

    public void buildTower(towerID id)
    {
        //Instantiate tower by id
        switch (id)
        {
            case towerID.Archer:
                GameObject tower = Instantiate(_ArcherTower, transform.position, Quaternion.identity);
                Debug.Log("_towerModel local: " + tower.transform.Find("building_archeryrange_blue").localPosition);
                Debug.Log("_front local: " + tower.transform.Find("building_archeryrange_blue/Front").localPosition);
                Debug.Log("_front world: " + tower.transform.Find("building_archeryrange_blue/Front").position);
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
