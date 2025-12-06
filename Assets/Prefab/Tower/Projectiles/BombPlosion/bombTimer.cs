using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bombTimer : MissileBase
{
    [SerializeField] private ParticleSystem SparksVfx;
    [SerializeField] private Transform detonationCord;

    [SerializeField] private GameObject Xplosion;

    [SerializeField] private bool PlayOnStart = true;
    private bool isDetonating = false;

    private float _blastRadius;
    private float _stun;

    private void Start()
    {
        if(PlayOnStart) isDetonating = true;
    }

    //call this function to detonate the bomb
    public void Detonate()
    {
        isDetonating = true;
    }

    protected override void Update()
    {
        if(isDetonating)
        {
            if(!SparksVfx.isPlaying) SparksVfx.Play();

            detonationCord.position = Vector3.Lerp(detonationCord.position, detonationCord.position - new Vector3(0, 0.2f, 0), 0.1f * Time.deltaTime);
        }

        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private object Explode()
    {
        SparksVfx.Stop();
        isDetonating = false;
        HashSet<EnemyController> processed = new ();

        foreach (Collider hit in Physics.OverlapSphere(gameObject.transform.position,_blastRadius, _enemyLayer))
        {
            EnemyController enemy = hit.GetComponentInParent<EnemyController>();

            if (enemy == null) continue;
            if (processed.Contains(enemy)) continue;

            processed.Add(enemy);
            enemy.TakeDamage(_damage, _type);
            enemy.TakeStun(_stun);
            SoundManager.instance.PlaySound(Sounds.Explode);
        }
        Destroy(gameObject);
        return Instantiate(Xplosion, detonationCord.position, transform.rotation);
    }

    public void Initialize(Transform destination, int damage, float blastRadius, float stun)
    {
        base.Initialize(destination, damage);
        _blastRadius = blastRadius;
        _stun = stun;
    }
}
