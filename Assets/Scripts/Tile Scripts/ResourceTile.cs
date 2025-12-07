using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceTile : BuildableTile
{
    public ResourceType type;
    public string tileName;
    public GameObject _builtPrefab;

    public static event Action<Resources> resourceBuilt;

    public void buildResource(towerID id)
    {
        GameObject tower = Instantiate(_builtPrefab, this.WorldPosition, Quaternion.identity);

        resourceBuilt?.Invoke(TowerDatabase.GetTowerStatic(id).cost);
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

    public new void Highlight(Color color)
    {

    }

    public new void ResetHighlight()
    {

    }
}
