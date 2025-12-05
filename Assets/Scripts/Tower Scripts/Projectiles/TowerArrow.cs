using UnityEngine;

//Specific behavior for arrows
public class TowerArrow : MissileBase
{
    protected override void ImpactEnemy(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage, _type);
        }

        base.ImpactEnemy(other);
    }
}
