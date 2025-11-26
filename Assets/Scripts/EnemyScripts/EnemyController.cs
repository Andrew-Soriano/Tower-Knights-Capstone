using System;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyController : MonoBehaviour
{
    private SplineAnimate _spline_anim;
    [SerializeField] private Animator _animator;
    private int _anim_attack;
    private int _anim_death;

    public float speed = 3;

    [SerializeField] private int _maxHP = 30;
    [SerializeField] private int _HP;

    public event Action onDeath;

    public float Path_Progress { get { return _spline_anim.NormalizedTime; } }

    void Awake()
    {
        _spline_anim = GetComponent<SplineAnimate>(); //Get animation component
        _anim_attack = Animator.StringToHash("Attack");
        _anim_death = Animator.StringToHash("Death");

        _HP = _maxHP;
    }

    public void startPath(SplineContainer path)
    {
        _spline_anim.Container = path;
        _spline_anim.Loop = SplineAnimate.LoopMode.Once;
        _spline_anim.Duration = _spline_anim.Container.CalculateLength()/speed;

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

    public void Damage(int damage)
    {
        _HP -= damage;

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
