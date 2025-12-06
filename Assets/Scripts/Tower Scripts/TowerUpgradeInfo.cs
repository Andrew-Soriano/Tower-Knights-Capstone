using System;
using System.Collections.Generic;
using UnityEngine;

public enum towerID
{
    Archer,
    Catapult,
    Bomb,
    Alchemist,
    Lumberyard,
    Quarry,
    Mine,
    Sawmill,
    Mason,
    Blacksmith
}

public enum UpgradeType
{
    ShootSpeed,
    ShotDamage,
    SlowEnemy,
    FireArrows,
    ShotPierce,
    Spikes,
    BlastRadius,
    Stun,
    EffectUp,
    Permanence,
    Vulnerability,
    FrostToFire,
    FasterProduction,
    MoreProduction,
    ProduceParts
}

[Serializable]
public class TowerInfo
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

[Serializable]
public class UpgradeInfo
{
    public UpgradeType type;
    public int maxLevel;
    public string name;
    public string description;
    public Sprite icon;
}

[Serializable]
public class UpgradeLevelCosts
{
    public UpgradeInfo upgrade;
    public List<Resources> levelCosts;
}

[CreateAssetMenu(fileName = "TowerUpgradeInfo", menuName = "Scriptable Objects/TowerUpgradeInfo")]
public class TowerUpgradeInfo : ScriptableObject
{
    public towerID id;
    public string towerName;
    public string description;
    public Sprite towerIcon;
    public Resources cost;

    [Header("Upgrades")]
    public List<UpgradeLevelCosts> upgrades;
}
