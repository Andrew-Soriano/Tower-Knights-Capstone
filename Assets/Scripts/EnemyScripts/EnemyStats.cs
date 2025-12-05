using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
    Orc,
    Werewolf,
    BlackKnight,
    Tiefling,
    Vampire,
    FrostGolem
}

public enum DamageType
{
    Pierce,
    Blunt,
    Fire,
    Blast
}

public class EnemyStats
{
    private int _maxHP;
    private int _damage;
    private float _speed;
    private float _stunReduction;

    private Dictionary<DamageType, float> _damageResistances = new Dictionary<DamageType, float>();

    public Dictionary<DamageType, float> Resistances { get => _damageResistances;}
    public int MaxHP { get => _maxHP; }
    public int Damage { get => _damage; }
    public float Speed { get => _speed; }
    public float StunReduction { get => _stunReduction; }

    public EnemyStats(int HP, int damage, float speed, float stunReduction, Dictionary<DamageType, float> resistances)
    {
        _maxHP = HP;
        _damage = damage;
        _speed = speed;
        _stunReduction = stunReduction;

        foreach(var kvp in resistances)
        {
            _damageResistances[kvp.Key] = Mathf.Clamp(kvp.Value, -1f, 1f);
        }
    }
}
