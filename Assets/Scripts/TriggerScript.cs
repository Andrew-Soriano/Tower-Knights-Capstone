using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject parent;
    private CastleController _parent_controller;
    public int id = 0;

    private void Awake()
    {
        _parent_controller = parent.GetComponent<CastleController>();
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.gameObject.GetComponent<EnemyController>().trigger(id);
        _parent_controller.trigger(id);
    }
}
