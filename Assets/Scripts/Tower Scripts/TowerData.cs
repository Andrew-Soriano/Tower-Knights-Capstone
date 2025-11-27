using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public static readonly Resources ArcherCost = new Resources(wood: 10);
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
}
