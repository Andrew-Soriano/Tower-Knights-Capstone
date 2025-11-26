using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject parent;
    private CastleController _paranet_controller;
    public int id = 0;

    private void Awake()
    {
        _paranet_controller = parent.GetComponent<CastleController>();
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        other.gameObject.GetComponent<EnemyController>().trigger(id);
        _paranet_controller.trigger(id);
    }
}
