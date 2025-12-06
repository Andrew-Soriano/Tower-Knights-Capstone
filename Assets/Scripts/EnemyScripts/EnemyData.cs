using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public static readonly EnemyStats skeleton = new EnemyStats(30, 1, 2, 0.2f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, 0f },
            {DamageType.Pierce, .1f },
            {DamageType.Fire, -.3f},
            {DamageType.Blast, -.1f },
            {DamageType.Frost, .3f }
        });
    public static readonly EnemyStats orc = new EnemyStats(10, 1, 3, 0.1f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, 0f },
            {DamageType.Pierce, -.1f },
            {DamageType.Fire, 0f},
            {DamageType.Blast, 0f },
            {DamageType.Frost, 0f }
        });
    public static readonly EnemyStats werewolf = new EnemyStats(10, 1, 6, 0.1f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, 0f },
            {DamageType.Pierce, 0f },
            {DamageType.Fire, -.1f},
            {DamageType.Blast, 0f },
            {DamageType.Frost, 0f }
        });
    public static readonly EnemyStats blackKnight = new EnemyStats(60, 10, 2, 0.3f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, .5f },
            {DamageType.Pierce, .2f },
            {DamageType.Fire, .5f},
            {DamageType.Blast, -.3f },
            {DamageType.Frost, -.5f }
        });
    public static readonly EnemyStats tiefling = new EnemyStats(30, 2, 4, 0.1f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, 0f },
            {DamageType.Pierce, 0f },
            {DamageType.Fire, 1f},
            {DamageType.Blast, -.2f },
            {DamageType.Frost, -1f }
        });
    public static readonly EnemyStats vampire = new EnemyStats(60, 2, 3, 0.3f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, .2f },
            {DamageType.Pierce, 0f },
            {DamageType.Fire, -.1f},
            {DamageType.Blast, 0f },
            {DamageType.Frost, .1f }
        });
    public static readonly EnemyStats frostGolem = new EnemyStats(100, 5, 2, 0.5f,
        new Dictionary<DamageType, float>{
            { DamageType.Blunt, .4f },
            {DamageType.Pierce, .4f },
            {DamageType.Fire, -1f},
            {DamageType.Blast, 0f },
            {DamageType.Frost, 1f }
        });

    public static EnemyStats GetStats(EnemyType type)
    {
        return type switch
        {
            EnemyType.Skeleton => skeleton,
            EnemyType.Orc => orc,
            EnemyType.Werewolf => werewolf,
            EnemyType.BlackKnight => blackKnight,
            EnemyType.Vampire => vampire,
            EnemyType.Tiefling => tiefling,
            EnemyType.FrostGolem => frostGolem,
            _ => throw new System.ArgumentException($"Unknown EnemyType: {type}")
        };
    }
}
