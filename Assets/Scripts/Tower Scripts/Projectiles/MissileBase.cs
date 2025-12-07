using UnityEngine;

public abstract class MissileBase : AmmoBase
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float rotateSpeed = 10f;

    protected virtual void Update()
    {
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;
        }

        MoveTowards(_targetPosition);
    }

    protected void MoveTowards(Vector3 dest)
    {
        Vector3 dir = (dest - transform.position);
        dir.y = 0;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        if (_targetTransform == null && Vector3.Distance(transform.position, dest) < 0.2f)
        {
            Impact();
        }
    }

    void Impact()
    {
        Destroy(gameObject);
    }

    protected virtual void ImpactEnemy(Collider other)
    {
        Impact();
    }

    private void OnTriggerEnter(Collider other)
    {
        ImpactEnemy(other);
    }
}
