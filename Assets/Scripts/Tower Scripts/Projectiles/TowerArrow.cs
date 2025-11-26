using UnityEngine;

//Specific behavior for arrows
public class TowerArrow : MissileBase
{
    [SerializeField] private int _damage = 10;

    protected override void ImpactEnemy(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Damage(_damage);
        }

        base.ImpactEnemy(other);
    }
}
