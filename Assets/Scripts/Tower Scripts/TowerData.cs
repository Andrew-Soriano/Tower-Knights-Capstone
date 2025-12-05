using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    ShootSpeed,
    ShotDamage,
    SlowEnemy,
    FireArrows,
    ShotPierce,
    Spikes,
    BlastRadius,
    Stun
}
public struct TowerInfo
{
    public string name;
    public string description;
    public Resources cost;

    public TowerInfo(string n, string d, Resources c)
    {
        name = n;
        description = d;
        cost = c;
    }
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
    public static readonly TowerInfo ArcherInfo = new("Archer", "Fires arrows to attack enemies with precision.", ArcherCost);

    public static readonly Resources CatapultCost = new Resources(wood: 10);
    public static readonly Resources Catapult_ShootSpeedCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShootSpeedCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShootSpeedCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotDamageCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotDamageCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotDamageCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotPierceCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotPierceCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_ShotPierceCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Catapult_SpikesCost = new Resources(planks: 10, bricks: 50, parts: 10);
    public static readonly Dictionary<UpgradeType, List<Resources>> CatapultUpgrades = new()
    {
        { UpgradeType.ShootSpeed, new List<Resources> { Catapult_ShootSpeedCost_1, Catapult_ShootSpeedCost_2, Catapult_ShootSpeedCost_3 } },
        { UpgradeType.ShotDamage, new List<Resources> { Catapult_ShotDamageCost_1, Catapult_ShotDamageCost_2, Catapult_ShotDamageCost_3 } },
        { UpgradeType.ShotPierce, new List<Resources> { Catapult_ShotPierceCost_1, Catapult_ShotPierceCost_2, Catapult_ShotPierceCost_3} },
        { UpgradeType.Spikes, new List<Resources> { Catapult_SpikesCost } }
    };
    public static readonly TowerInfo CatapultInfo = new("Catapult", "Fires boulders that roll over enemies in a straight line", CatapultCost);

    public static readonly Resources BombCost = new Resources(wood: 10);
    public static readonly Resources Bomb_ShootSpeedCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShootSpeedCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShootSpeedCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotDamageCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotDamageCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotDamageCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotBlastRadiusCost_1 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotBlastRadiusCost_2 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_ShotBlastRadiusCost_3 = new Resources(wood: 10, planks: 10, stone: 10);
    public static readonly Resources Bomb_StunCost = new Resources(planks: 10, bricks: 50, parts: 10);
    public static readonly Dictionary<UpgradeType, List<Resources>> BombUpgrades = new()
    {
        { UpgradeType.ShootSpeed, new List<Resources> { Bomb_ShootSpeedCost_1, Bomb_ShootSpeedCost_2, Bomb_ShootSpeedCost_3 } },
        { UpgradeType.ShotDamage, new List<Resources> { Bomb_ShotDamageCost_1, Bomb_ShotDamageCost_2, Bomb_ShotDamageCost_3 } },
        { UpgradeType.BlastRadius, new List<Resources> { Bomb_ShotBlastRadiusCost_1, Bomb_ShotBlastRadiusCost_2, Bomb_ShotBlastRadiusCost_3} },
        { UpgradeType.Stun, new List<Resources> { Bomb_StunCost } }
    };
    public static readonly TowerInfo BombInfo = new("Bomb", "Fires boulders that roll over enemies in a straight line", BombCost);

    public static readonly Resources SawmillCost = new Resources(wood: 10, parts: 10);
    public static readonly TowerInfo SawmillInfo = new("Sawmill", "Turns wood into planks.", SawmillCost);

    public static Resources GetCosts(towerID id)
    {
        return id switch
        {
            towerID.Archer => ArcherCost,
            towerID.Catapult => CatapultCost,
            towerID.Bomb => BombCost,
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

    public static List<Resources> GetUpgradeLevelsCosts(towerID id, UpgradeType type)
    {
        return id switch
        {
            towerID.Archer => ArcherUpgrades[type],
            towerID.Catapult => CatapultUpgrades[type],
            towerID.Bomb => BombUpgrades[type],
            _ => throw new System.ArgumentException($"Unknown towerID: {id}")
        };
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
            "Pierce" => UpgradeType.ShotPierce,
            "Spiked" => UpgradeType.Spikes,
            "Blast Radius" => UpgradeType.BlastRadius,
            "Stun" => UpgradeType.Stun,
            _ => throw new ArgumentException($"Unknown action name: {name}")
        };
    }

    public static TowerInfo GetTowerInfo(towerID id)
    {
        return id switch
        {
            towerID.Archer => ArcherInfo,
            towerID.Catapult => CatapultInfo,
            towerID.Bomb => BombInfo,
            towerID.Sawmill => SawmillInfo,
            _ => throw new System.ArgumentException($"Unknown towerID: {id}")
        };
    }
}
