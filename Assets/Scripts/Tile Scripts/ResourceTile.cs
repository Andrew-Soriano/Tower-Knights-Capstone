using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceTile : BuildableTile
{
    public ResourceType type;
    public int amountPerRound;
    [HideInInspector] public string tileName;

    private bool _built = false;

    private void OnEnable()
    {
        UIManager.nextRoundClicked += AddResource;
    }
    private void OnDisable()
    {
        UIManager.nextRoundClicked -= AddResource;
    }

    public override void OnSelect()
    {
        UIManager.instance.OpenResourceMenu(this);
        base.OnSelect();
    }

    public override void OnDeselect()
    {
        UIManager.instance.CloseMenu();
        base.OnSelect();
    }

    public void BuildResource()
    {
        if (!_built)
        {
            _built = true;
            _switchMode(1);
        }
    }

    private void AddResource()
    {
        if (_built)
        {
            CastleController.instance.stockpile.Add(type, amountPerRound);
            UIManager.instance.RefreshResourceUI(CastleController.instance.stockpile);
        }
    }
}
