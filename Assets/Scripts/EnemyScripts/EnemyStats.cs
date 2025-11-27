using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
}

public enum DamageType
{
    Pierce,
    Fire
}

public class EnemyStats
{
    private int _maxHP;
    private float _speed;

    private Dictionary<DamageType, float> _damageResistances = new Dictionary<DamageType, float>();

    public Dictionary<DamageType, float> Resistances { get => _damageResistances;}
    public int MaxHP { get => _maxHP; }
    public float Speed { get => _speed; }


    public EnemyStats(int HP, float Speed, float resist_pierce)
    {
        _maxHP = HP;
        _speed = Speed;
        _damageResistances[DamageType.Pierce] = Mathf.Clamp(resist_pierce, 0f, 1f);
    }
}
