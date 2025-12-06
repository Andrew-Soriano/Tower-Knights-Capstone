using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Scriptable Objects/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    private static TowerDatabase _instance;
    public List<TowerUpgradeInfo> towers;

    private Dictionary<towerID, TowerUpgradeInfo> _lookup;

    public static TowerDatabase Instance {get => _instance;}

    public void InitLookup()
    {
        _lookup = new Dictionary<towerID, TowerUpgradeInfo>();
        foreach (var tower in towers)
        {
            if (!_lookup.ContainsKey(tower.id))
                _lookup.Add(tower.id, tower);
        }
    }

    public TowerUpgradeInfo GetTower(towerID id)
    {
        if (_lookup == null) InitLookup();
        _lookup.TryGetValue(id, out var tower);
        if(tower == null)
        {
            Debug.LogWarning($"No TowerData found for {id}");
        }
        return tower;
    }

    public static TowerUpgradeInfo GetTowerStatic(towerID id)
    {
        return Instance.GetTower(id);
    }
    public static void SetInstance(TowerDatabase db)
    {
        _instance = db;
        _instance.InitLookup();
    }
}
