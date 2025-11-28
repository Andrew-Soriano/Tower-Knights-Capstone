using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    ShootSpeed,
    ShotDamage,
    SlowEnemy,
    FireArrows
}

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public static readonly Resources ArcherCost = new Resources(wood: 10);
    public static readonly Resources Archer_ShootSpeedCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_ShootSpeedCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_ShootSpeedCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_ShotDamageCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_ShotDamageCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_ShotDamageCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Archer_SlowEnemyCost = new Resources(planks: 10, stone: 10, metal: 10);
    public static readonly Resources Archer_FireArrowsCost = new Resources(planks: 10, bricks: 50, parts: 10);
    public static readonly Dictionary<UpgradeType, List<Resources>> ArcherUpgrades = new()
    {
        { UpgradeType.ShootSpeed, new List<Resources> { Archer_ShootSpeedCost_1, Archer_ShootSpeedCost_2, Archer_ShootSpeedCost_3 } },
        { UpgradeType.ShotDamage, new List<Resources> { Archer_ShotDamageCost_1, Archer_ShotDamageCost_2, Archer_ShotDamageCost_3 } },
        { UpgradeType.SlowEnemy, new List<Resources> { Archer_SlowEnemyCost } },
        { UpgradeType.FireArrows, new List<Resources> { Archer_FireArrowsCost } }
    };

    public static readonly Resources SawmillCost = new Resources(wood: 10, parts: 10);

    public static Resources GetCosts(towerID id)
    {
        return id switch
        {
            towerID.Archer => ArcherCost,
            towerID.Sawmill => SawmillCost,
            _ => throw new System.ArgumentException($"Unknown towerID: {id}")
        };
    }

    public static Resources GetUpgradeCost(towerID id, UpgradeType type, int level)
    {
        if (id == towerID.Archer)
        {
            var list = ArcherUpgrades[type];
            if (level >= 0 && level < list.Count)
                return list[level];
            return null; // no further upgrades
        }

        return null;
    }

    public static int GetMaxLevels(towerID id, UpgradeType type)
    {
        if (id == towerID.Archer)
            return ArcherUpgrades[type].Count;
        return 0;
    }
    public static UpgradeType MapStringToUpgradeType(string name)
    {
        return name switch
        {
            "Shoot Speed" => UpgradeType.ShootSpeed,
            "Shot Damage" => UpgradeType.ShotDamage,
            "Slow Enemy" => UpgradeType.SlowEnemy,
            "Fire Arrows" => UpgradeType.FireArrows,
            _ => throw new ArgumentException($"Unknown action name: {name}")
        };
    }
}
