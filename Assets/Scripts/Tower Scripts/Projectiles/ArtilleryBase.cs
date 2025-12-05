using UnityEngine;

public abstract class ArtilleryBase : AmmoBase
{
    [SerializeField] private float travelTime = 1.2f;
    [SerializeField] private float arcHeight = 3f;
    protected bool inAir = false;

    protected Vector3 _startPosition;
    protected float _t;

    void Start()
    {
    }

    protected virtual void Update()
    {
        if (!inAir) return;
        _t += Time.deltaTime / travelTime;

        // Get linear horizontal movement
        Vector3 linearPos = Vector3.Lerp(_startPosition, _targetPosition, _t);

        // Add arc (parabola)
        float height = arcHeight * (1f - Mathf.Pow(_t * 2 - 1, 2));
        linearPos.y += height;

        transform.position = linearPos;

        // Stop in-air when we reach target
        //if (_t >= 1f)
            //inAir = false;
    }
}
