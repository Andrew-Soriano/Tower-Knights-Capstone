using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyController : MonoBehaviour
{
    private SplineAnimate _spline_anim;
    [SerializeField] private Animator _animator;
    private int _anim_attack;
    private int _anim_death;

    [SerializeField] private EnemyType type;
    private float _speed;

    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    private Dictionary<DamageType, float> _damageResistances;

    public event Action onDeath;

    public float Path_Progress { get { return _spline_anim.NormalizedTime; } }

    void Awake()
    {
        _spline_anim = GetComponent<SplineAnimate>(); //Get animation component
        _anim_attack = Animator.StringToHash("Attack");
        _anim_death = Animator.StringToHash("Death");

        EnemyStats stats = EnemyData.GetStats(type);
        _maxHP = stats.MaxHP;
        _HP = _maxHP;
        _speed = stats.Speed;
        _damageResistances = stats.Resistances;
    }

    public void startPath(SplineContainer path)
    {
        _spline_anim.Container = path;
        _spline_anim.Loop = SplineAnimate.LoopMode.Once;
        _spline_anim.Duration = _spline_anim.Container.CalculateLength()/_speed;

        _spline_anim.Play();
    }

    public void trigger(int id)
    {
        _animator.SetTrigger(_anim_attack);
    }

    public void Die()
    {
        onDeath?.Invoke();
        onDeath = null;
        Destroy(this.transform.gameObject);
    }

    public void Damage(int damage, DamageType type)
    {
        _HP -= (int) (damage * 1f - _damageResistances[type]);

        if (_HP <= 0)
        {
            _animator.SetTrigger(_anim_death);
        }
    }

    public bool isDead()
    {
        return _HP <= 0;
    }
}
