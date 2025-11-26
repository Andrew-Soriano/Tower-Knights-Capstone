using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
}

public enum DamageType
{
    Pierce
}

public class EnemyStats
{
    private int _maxHP;

    private Dictionary<DamageType, float> _damageResistances = new Dictionary<DamageType, float>();

    public Dictionary<DamageType, float> Resistances { get => _damageResistances;}

    public EnemyStats(int HP, float resist_pierce)
    {
        _maxHP = HP;
        _damageResistances[DamageType.Pierce] = Mathf.Clamp(resist_pierce, 0f, 1f);
    }

    public int Damage(DamageType type, int damage)
    {
        return (int)(1f - _damageResistances[type] * damage);
    }
}
