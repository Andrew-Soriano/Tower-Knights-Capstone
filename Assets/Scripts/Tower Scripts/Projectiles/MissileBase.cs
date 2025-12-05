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
        //Move
        Vector3 dir = (dest - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Rotate to face movement
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }

        // Reached?
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
