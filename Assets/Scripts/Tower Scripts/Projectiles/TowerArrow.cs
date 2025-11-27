using UnityEngine;

//Specific behavior for arrows
public class TowerArrow : MissileBase
{
    protected override void ImpactEnemy(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Damage(_damage, _type);
        }

        base.ImpactEnemy(other);
    }
}
