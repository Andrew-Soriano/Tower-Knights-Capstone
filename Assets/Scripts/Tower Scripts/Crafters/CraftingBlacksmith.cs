using System;
using UnityEngine;

public class CratingBlacksmith : CraftingBase
{
    protected override void Awake()
    {
        base.Awake();
        if (type != ResourceType.Metal && type != ResourceType.Parts)
            type = ResourceType.Metal;
    }

    protected override Action GetUpgradeAction(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.MoreProduction => UpgradeMoreProduction,
            UpgradeType.FasterProduction => UpgradeFasterProduction,
            UpgradeType.ProduceParts => UpgradeProduceParts,
            _ => base.GetUpgradeAction(type)
        };
    }


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

    private void UpgradeFasterProduction()
    {
        amountPerRound += 10;
    }

    private void UpgradeMoreProduction()
    {
        multiplier++;
    }

    public void UpgradeProduceParts()
    {
        amountPerRound /= 2;
        type = ResourceType.Parts;
    }
}
