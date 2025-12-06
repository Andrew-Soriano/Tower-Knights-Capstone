using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private GameObject _lightningPrefab;
    [SerializeField] private Transform _startLightning;
    [SerializeField] private float _lightningDuration = 0.2f;
    [SerializeField] private float _lightningJaggedness = 0.5f;
    [SerializeField] private int _lightningSegments = 8;

    public void Strike(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        LightningBolt lightning = Instantiate(_lightningPrefab).GetComponent<LightningBolt>();
        lightning.Initialize(_startLightning, other.transform, _lightningSegments, _lightningJaggedness, _lightningDuration);
    }
}
