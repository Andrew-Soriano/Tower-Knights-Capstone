using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TowerFlask : ArtilleryBase
{
    private Rigidbody rb;
    private float _slow;
    private float _persist;
    private float _vulnerability;
    private UnBuildableTile _targetTile;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _targetTile.Initialize(_slow, _damage, _type, _persist, _vulnerability);

            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 target, int damage, float slow, float persist, float vulnerability, Vector3 pos, UnBuildableTile tile)
    {
        _t = 0f;
        _startPosition = pos;
        _slow = slow;
        _persist = persist;
        _vulnerability = vulnerability;
        _targetTile = tile;

        base.Initialize(target, damage);
        inAir = true;
    }
}
