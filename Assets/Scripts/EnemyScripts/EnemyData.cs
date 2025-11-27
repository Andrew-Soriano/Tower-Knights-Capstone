using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public static readonly EnemyStats skeleton = new EnemyStats(30, 3, .5f);

    public static EnemyStats GetStats(EnemyType type)
    {
        return type switch
        {
            EnemyType.Skeleton => skeleton,
            _ => throw new System.ArgumentException($"Unknown EnemyType: {type}")
        };
    }
}
