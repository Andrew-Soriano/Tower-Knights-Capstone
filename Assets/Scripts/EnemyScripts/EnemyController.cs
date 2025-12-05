using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    private Coroutine _slowResetCoroutine;

    private float _stun = 0;
    [SerializeField, Range(0f, 1f)]
    private float _stunReduction;
    private float _stunMultiplier = 1f;

    private int _maxHP;
    private int _HP;
    private Dictionary<DamageType, float> _damageResistances;

    [SerializeField] private int _damage;

    public event Action onDeath;

    public float Path_Progress { get { return _spline_anim.NormalizedTime; } }
    public int Damage { get => _damage; }

    void Awake()
    {
        _spline_anim = GetComponent<SplineAnimate>(); //Get animation component
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
    }

    public void startPath(SplineContainer path)
    {
        _spline_anim.Container = path;
        _spline_anim.Loop = SplineAnimate.LoopMode.Once;
        _spline_anim.MaxSpeed = _speed;

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

    public void TakeDamage(int damage, DamageType type)
    {
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

        _slow = Mathf.Min(_slow + slow, _speed * 0.75f); ;

        _spline_anim.MaxSpeed = _speed - _slow;
        _slowResetCoroutine = StartCoroutine(SlowRecovery(5f));
    }

    private IEnumerator SlowRecovery(float delay)
    {
        yield return new WaitForSeconds(delay);
        _slow = 0f;
        _slowResetCoroutine = null;
        _spline_anim.MaxSpeed = _speed;
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

        _spline_anim.Pause();
        Vector3 originalPosition = transform.position;

        while (_stun > 0)
        {
            transform.position = originalPosition + new Vector3(Random.Range(-.5f, .5f) * magnitude,
                                                                       0,
                                                                       0);

            _stun -= Time.deltaTime;
            _animator.SetFloat(_anim_stun, _stun);
            yield return null;
        }

        transform.position = originalPosition;
        _spline_anim.Play();
    }

    public bool isDead()
    {
        return _HP <= 0;
    }
}
