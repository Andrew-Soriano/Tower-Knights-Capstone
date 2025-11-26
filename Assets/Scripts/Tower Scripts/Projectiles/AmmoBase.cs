using UnityEngine;

public abstract class AmmoBase : MonoBehaviour
{
    protected Transform _targetTransform;    // For homing
    protected Vector3 _targetPosition;       //For firing at a target position
    protected bool _hasTransformTarget;

    public virtual void Initialize(Transform destination)
    {
        _targetTransform = destination;
        _targetPosition = destination.position;
        _hasTransformTarget = true;
    }

    public virtual void Initialize(Vector3 destination)
    {
        _targetPosition = destination;
        _hasTransformTarget = false;
    }
}
