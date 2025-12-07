using System;
using UnityEngine;

public class CraftingBase : BuildingBase
{
    public int amountPerRound;
    public int multiplier = 1;
    public ResourceType type;
    
    private void OnEnable()
    {
        StatusBarController.nextRoundClicked += AddResource;
    }
    private void OnDisable()
    {
        StatusBarController.nextRoundClicked -= AddResource;
    }
    
    private void AddResource()
    {
        CastleController.instance.AddResourceToStock(type, amountPerRound * multiplier);
    }
    
    protected override Action GetUpgradeAction(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.MoreProduction => UpgradeMoreProduction,
            UpgradeType.FasterProduction => UpgradeFasterProduction,
            _ => base.GetUpgradeAction(type)
        };
    }
    
    private void UpgradeFasterProduction()
    {
        amountPerRound += 10;
    }
    
    private void UpgradeMoreProduction()
    {
        multiplier++;
    }
}
