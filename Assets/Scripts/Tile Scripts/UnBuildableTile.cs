using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UnBuildableTile : MonoBehaviour, IClickable, ISelectable
{
    [SerializeField] private Renderer _tileRenderer;
    [SerializeField] private ParticleSystem _frostParticles;
    [SerializeField] private ParticleSystem _fireParticles;
    [SerializeField] private LayerMask _enemyLayer;

    private Color originalColor;
    private Coroutine _AoERoutine;

    private float _duration = 5f;
    private float _radius = 2f;
    private float _tickInterval = 0.5f;

    private bool _enabled = false;
    private float _slow = 0;
    private int _damagePerTick = 0;

    private DamageType _type = DamageType.Pierce;
    private Dictionary<DamageType, ParticleSystem> _particles;

    private float _persist;
    private float _vulnerability;

    void Awake()
    {
        if (!Application.isPlaying) return;
        if (_tileRenderer == null)
        {
            Debug.LogError($"{name} is missing _tileRenderer!", this);
            return;
        }
        originalColor = _tileRenderer.material.color;
        _particles = new Dictionary<DamageType, ParticleSystem>
        {
            {DamageType.Fire, _fireParticles },
            {DamageType.Frost, _frostParticles }
        };
    }

    public void Highlight(Color color)
    {
        if (!Application.isPlaying) return;
        _tileRenderer.material.color = color;
    }

    public void ResetHighlight()
    {

        if (!Application.isPlaying) return;
        _tileRenderer.material.color = originalColor;
    }

    public void OnClicked() => SelectionManager.instance.Select(this);

    public virtual void OnSelect(){}

    public virtual void OnDeselect(){}

    public void Initialize(float slow, int damage, DamageType type, float persist, float vulnerability)
    {
        StopAoE();

        if (!ShouldAcceptType(type)) return;

        _slow = slow;
        _damagePerTick = damage;
        _type = type;
        _persist = persist;
        _vulnerability = vulnerability;

        PlayParticles(type);

        _enabled = true;
        _AoERoutine = StartCoroutine(AOECoroutine());
    }
    private IEnumerator AOECoroutine()
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _enemyLayer);
            HashSet<EnemyController> enemiesHit = new();

            foreach (var hit in hits)
            {
                var enemy = hit.GetComponentInParent<EnemyController>();
                if (enemy != null)
                {
                    if (enemiesHit.Contains(enemy)) continue;
                    enemiesHit.Add(enemy);
                }
            }

            foreach (var enemy in enemiesHit)
            {
                enemy.ApplyTileEffect(_damagePerTick, _slow, _persist);
            }

            elapsed += _tickInterval;
            yield return new WaitForSeconds(_tickInterval);
        }

        StopAoE();
    }

    private void StopAoE()
    {
        if (!_enabled)
            return;

        if (_particles.TryGetValue(_type, out var ps) && ps.isPlaying)
            ps.Stop();

        if (_AoERoutine != null)
            StopCoroutine(_AoERoutine);

        _type = DamageType.Pierce;
        _enabled = false;
        _AoERoutine = null;
        _persist = 0;
        _slow = 0;
        _damagePerTick = 0;
        _vulnerability = 0;
    }

    private bool ShouldAcceptType(DamageType type)
    {
        if (_type == DamageType.Fire && type == DamageType.Frost)
            return false;

        if (_type == DamageType.Frost && type == DamageType.Fire)
            return false;

        return true;
    }
    private void PlayParticles(DamageType type)
    {
        if (_particles.TryGetValue(type, out var ps))
        {
            ps.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
