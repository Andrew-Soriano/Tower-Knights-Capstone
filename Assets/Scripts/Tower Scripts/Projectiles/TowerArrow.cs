using UnityEngine;

public class TowerArrow : MissileBase
{
    float _slow = 0f;
    protected override void ImpactEnemy(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage, _type);
            enemy.TakeSlow(_slow);
            SoundManager.instance.PlaySound(Sounds.ArrowImpact);
        }

        base.ImpactEnemy(other);
    }

    public void Initialize(Transform target, int damage, float slow)
    {
        base.Initialize(target, damage);
        _slow = slow;
    }
}
