using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Boulder : ArtilleryBase
{
    private Rigidbody rb;
    private bool onGround = false;
    [SerializeField] private float groundSpeed = 5f;
    [SerializeField] private float _radius = 0.5f;
    private Vector3 moveDirection;
    private int _pierce;

    [Header("Auto-Destruction Settings")]
    [SerializeField] private float maxRollDistance = 25f;
    [SerializeField] private float maxRollTime = 6f;

    private float rollTimer = 0f;
    private Vector3 rollStartPos;

    private HashSet<EnemyController> enemiesHit = new HashSet<EnemyController>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (onGround)
        {
            transform.position += moveDirection * groundSpeed * Time.deltaTime;
            Vector3 rollAxis = Vector3.Cross(Vector3.up, moveDirection);
            transform.Rotate(rollAxis, (groundSpeed * Time.deltaTime) / _radius * Mathf.Rad2Deg, Space.World);
            if (Vector3.Distance(rollStartPos, transform.position) >= maxRollDistance)
            {
                Destroy(gameObject);
                return;
            }

            rollTimer += Time.deltaTime;
            if (rollTimer >= maxRollTime)
            {
                Destroy(gameObject);
                return;
            }
        }


        base.Update();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!onGround && collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            inAir = false;

            moveDirection = Vector3.ProjectOnPlane(_targetPosition - _startPosition, Vector3.up).normalized;

            rollTimer = 0f;
            rollStartPos = transform.position;
        }

        if(onGround && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponentInParent<EnemyController>();
            if (enemy != null && !enemiesHit.Contains(enemy))
            {
                enemiesHit.Add(enemy);
                enemy.TakeDamage(_damage, _type);

                _pierce--;
                if (_pierce <= 0)
                    Destroy(this.gameObject);
            }
        }
    }

    public void Initialize(Vector3 target, int damage, int pierce, Vector3 pos)
    {
        _pierce = pierce;
        _t = 0f;
        _startPosition = pos;
        base.Initialize(target, damage);
        inAir = true;
        onGround = false;
    }
}
