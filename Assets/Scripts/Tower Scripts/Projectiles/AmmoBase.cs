using UnityEngine;

public abstract class AmmoBase : MonoBehaviour
{
    protected Transform _targetTransform;    // For homing
    protected Vector3 _targetPosition;       //For firing at a target position
    protected bool _hasTransformTarget;
    [SerializeField] protected DamageType _type;

    protected int _damage;

    public virtual void Initialize(Transform destination, int damage)
    {
        _targetTransform = destination;
        _targetPosition = destination.position;
        _hasTransformTarget = true;
        _damage = damage;
    }

    public virtual void Initialize(Vector3 destination, int damage)
    {
        _targetPosition = destination;
        _hasTransformTarget = false;
        _damage = damage;
    }
}
