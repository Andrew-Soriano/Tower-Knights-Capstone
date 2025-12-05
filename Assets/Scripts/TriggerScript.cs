using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject parent;
    private CastleController _parent_controller;
    private LightningStrike _lightning;
    public int id = 0;

    private HashSet<EnemyController> _alreadyTriggered = new HashSet<EnemyController>();

    private void Awake()
    {
        _parent_controller = parent.GetComponent<CastleController>();
        _lightning = GetComponent<LightningStrike>();
    }

    private void OnEnable()
    {
        UIManager.nextRoundClicked += ClearEnemyHash;
    }

    private void OnDisable()
    {
        UIManager.nextRoundClicked -= ClearEnemyHash;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponentInParent<EnemyController>();
            if (_alreadyTriggered.Contains(enemy))
                return;

            _alreadyTriggered.Add(enemy);
            enemy.trigger(id);
            _lightning.Strike(other);
            _parent_controller.trigger(enemy);
        }
    }

    public void ClearEnemyHash()
    {
        _alreadyTriggered.Clear();
    }
}
