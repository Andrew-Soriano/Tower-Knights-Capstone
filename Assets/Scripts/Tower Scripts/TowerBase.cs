using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerBase : MonoBehaviour, IClickable, ISelectable
{
    [Header("References")]
    [SerializeField] public Transform _front;
    [SerializeField] private GameObject _ammo;
    [SerializeField] private GameObject _towerModel;

    [Header("Firing")]
    [SerializeField] private float _fireAngle = 90f;
    [SerializeField] private float _fireRange = 5f;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private ArcRange _rangeArc;

    [Header("Layers")]
    [SerializeField] private LayerMask _enemyLayer;

    private float _fireCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _fireCooldown -= Time.deltaTime;

        Transform target = GetTargetInRange();

        if (target != null && _fireCooldown <= 0f)
        {
            FireAt(target);
            _fireCooldown = 1f / _fireRate;
        }
    }

    Transform GetTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _fireRange, _enemyLayer);

        float bestDist = float.MaxValue;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            //If enemy is dead, do not target
            if (hit.GetComponent<EnemyController>().isDead()) continue;

            Vector3 toTarget = hit.transform.position - transform.position;
            // Check angle
            if (Vector3.Angle(_front.forward, toTarget) > _fireAngle * 0.5f)
                continue;

            float dist = toTarget.sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }

    private void FireAt(Transform target)
    {
        Debug.Log("Firing");
        Instantiate(_ammo, _front.position, _front.rotation).GetComponent<AmmoBase>().Initialize(target);
    }

    public void RotateLeft()
    {
        Debug.Log("_front.forward at spawn: " + _front.forward);
        Debug.Log("_towerModel.forward at spawn: " + _towerModel.transform.forward);
        _towerModel.transform.rotation *= Quaternion.Euler(0f, -30f, 0f);
    }

    public void RotateRight()
    {
        Debug.Log("_front.forward at spawn: " + _front.forward);
        Debug.Log("_towerModel.forward at spawn: " + _towerModel.transform.forward);
        _towerModel.transform.rotation *= Quaternion.Euler(0f, 30f, 0f);
    }

    public void OnClicked()
    {
        SelectionManager.instance.Select(this);
    }

    public void OnSelect()
    {
        if (_rangeArc != null)
        {
            _rangeArc.gameObject.SetActive(true);

            // Update arc to match our firing parameters
            _rangeArc.SetArc(_fireRange, _fireAngle);

            // Rotate the arc to match the tower's forward direction
            //_rangeArc.transform.rotation = Quaternion.LookRotation(_front.forward, Vector3.up);

            //Fade it in
            _rangeArc.FadeIn();
        }

        UIManager.instance.OpenTowerMenu();
    }

    public void OnDeselect()
    {
            Debug.Log("Disabling Arc");
        if (_rangeArc != null)
        {
            _rangeArc.gameObject.SetActive(false);
        }

        UIManager.instance.CloseMenu();
    }
}
