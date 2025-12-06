using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private SplineAnimate _spline_anim;
    [SerializeField] private Animator _animator;
    private int _anim_attack;
    private int _anim_death;
    private int _anim_stun;

    [SerializeField] private EnemyType type;
    private float _speed;
    private float _slow = 0;
    private Coroutine _slowResetCoroutine = null;

    private float _stun = 0;
    [SerializeField, Range(0f, 1f)]
    private float _stunReduction;
    private float _stunMultiplier = 1f;

    private bool _moving = true;

    private int _maxHP;
    private int _HP;
    private Dictionary<DamageType, float> _damageResistances;
    private Dictionary<DamageType, float> _damageVulnerability;
    private int _doT;
    private Coroutine _persistResetCoroutine = null;
    private Coroutine _delayVulnerableReset = null;

    [SerializeField] private int _damage;

    public event Action onDeath;

    public float Path_Progress { get { return _spline_anim.NormalizedTime; } }
    public int Damage { get => _damage; }
    public EnemyType Type => type;

    void Awake()
    {
        _spline_anim = GetComponent<SplineAnimate>();
        _anim_attack = Animator.StringToHash("Attack");
        _anim_death = Animator.StringToHash("Death");
        _anim_stun = Animator.StringToHash("Stun");

        EnemyStats stats = EnemyData.GetStats(type);
        _maxHP = stats.MaxHP;
        _HP = _maxHP;
        _damage = stats.Damage;
        _speed = stats.Speed;
        _stunReduction = stats.StunReduction;
        _damageResistances = stats.Resistances;

        _damageVulnerability = new() {
            {DamageType.Blunt, 0f },
            {DamageType.Pierce, 0f },
            {DamageType.Blast, 0f }
        };
    }
    void Update()
    {
        if (!_moving) return;

        _spline_anim.NormalizedTime += (_speed * (1 - _slow)) * (Time.deltaTime / _spline_anim.Container.Spline.GetLength());

        _spline_anim.NormalizedTime = Mathf.Clamp01(_spline_anim.NormalizedTime);

        if (_spline_anim.NormalizedTime >= 1f)
            _moving = false;
    }

    public void startPath(SplineContainer path)
    {
        _spline_anim.Container = path;
        _spline_anim.Loop = SplineAnimate.LoopMode.Once;
        _spline_anim.Restart(false);
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

    public void TakeDamage(int damage, DamageType type)
    {
        Debug.Log(damage);
        _HP -= (int) (damage * (1f - _damageResistances[type]));

        if (_HP <= 0)
        {
            _animator.SetTrigger(_anim_death);
            _spline_anim.Pause();
            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }
    }

    public void TakeSlow(float slow)
    {
        if (_slowResetCoroutine != null)
            StopCoroutine(_slowResetCoroutine);

        _slow = slow;
        _slowResetCoroutine = StartCoroutine(SlowRecovery(3f));
    }

    private IEnumerator SlowRecovery(float delay)
    {
        yield return new WaitForSeconds(delay);
        _slow = 0f;
        _slowResetCoroutine = null;
        _spline_anim.MaxSpeed = _speed;
    }

    private void TakeVulnerable(float vulnerable)
    {
        foreach(var kvp in _damageVulnerability)
        {
            _damageVulnerability[kvp.Key] = vulnerable;
        }
    }

    private void ResetVulnerable()
    {
        foreach(var kvp in _damageVulnerability)
        {
            _damageVulnerability[kvp.Key] = 0f;
        }
    }

    private IEnumerator DelayedResetVulnerable()
    {
        yield return new WaitForSeconds(1);
        ResetVulnerable();
    }

    public void ApplyTileEffect(int damage = 0, float slow = 0, float persistTime= 0, float vulnerable = 0)
    {
        if (damage > 0) _doT = damage;
        if (slow > _slow) _slow = slow;

        TakeVulnerable(vulnerable);

        if (persistTime > 0)
        {
            if (_persistResetCoroutine != null) StopCoroutine(_persistResetCoroutine);
            _persistResetCoroutine = StartCoroutine(PersistCoroutine(persistTime));
        }
        else
        {
            if (_doT > 0) TakeDamage(_doT, DamageType.Fire);
            _doT = 0;

            TakeSlow(slow);

            if(_persistResetCoroutine != null)
            {
                if (_delayVulnerableReset != null)
                {
                    StopCoroutine(_delayVulnerableReset);
                    _delayVulnerableReset = null;
                }
                
                _delayVulnerableReset = StartCoroutine(DelayedResetVulnerable());
            }
        }
    }

    private IEnumerator PersistCoroutine(float persistTime)
    {
        if (_delayVulnerableReset != null)
        {
            StopCoroutine(_delayVulnerableReset);
            _delayVulnerableReset = null;
        }

        while (persistTime > 0f)
        {
            persistTime -= 0.5f;
            if (_doT > 0) TakeDamage(_doT, DamageType.Fire);
            if (_slow > 0) TakeSlow(_slow);
            yield return new WaitForSeconds(0.5f);
        }

        _doT = 0;
        _slow = 0;
        _persistResetCoroutine = null;
    }


    public void TakeStun(float stun)
    {
        if (_stun > 0)
            return;

        _stun = stun * _stunMultiplier;

        if (_stun <= 0)
            return;

        Shake();

        _stunMultiplier *= _stunReduction;
    }

    public void Shake(float magnitude = 0.2f)
    {
        StartCoroutine(DoShake(magnitude));
    }

    private IEnumerator DoShake(float magnitude)
    {

        _moving = false;
        Vector3 originalPosition = transform.position;

        while (_stun > 0)
        {
            transform.position = originalPosition + new Vector3(Random.Range(-.5f, .5f) * magnitude, 0, 0);

            _stun -= Time.deltaTime;
            _animator.SetFloat(_anim_stun, _stun);
            yield return null;
        }

        transform.position = originalPosition;
        _moving = true;
    }

    public bool isDead()
    {
        return _HP <= 0;
    }
}
