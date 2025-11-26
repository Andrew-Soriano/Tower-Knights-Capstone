using UnityEngine;

public abstract class ArtilleryBase : AmmoBase
{
    [SerializeField] private float travelTime = 1.2f;
    [SerializeField] private float arcHeight = 3f;

    private Vector3 _startPosition;
    private float _t;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        _t += Time.deltaTime / travelTime;

        // Get linear horizontal movement
        Vector3 linearPos = Vector3.Lerp(_startPosition, _targetPosition, _t);

        // Add arc (parabola)
        float height = arcHeight * (1f - Mathf.Pow(_t * 2 - 1, 2));
        linearPos.y += height;

        transform.position = linearPos;

        if (_t >= 1f)
            Impact();
    }

    void Impact()
    {
        Destroy(gameObject);
    }
}
