using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class EnemyBaseController : MonoBehaviour
{
    public static event Action endOfRound;
    public static event Action victory;
    [Header("Path")]
    public SplineContainer path;

    [Header("Enemies")]
    [SerializeField] EnemyWaves waves;
    private List<GameObject> Enemies;
    public float defaultSpawnDelay = 0.3f;

    public int roundNum = 0;

    private Coroutine _spawnRoutine = null;
    private int _enemiesSpawned = 0;
    private int _enemiesAlive = 0;

    private void OnEnable()
    {
        StatusBarController.nextRoundClicked += StartRound;
    }
    private void OnDisable()
    {
        StatusBarController.nextRoundClicked -= StartRound;
    }

    private IEnumerator SpawnEnemies()
    {
        int i = 0;
        var wave = waves.GetWave(roundNum - 1);

        foreach(var segment in wave.segments)
        {
            for (int j = 0; j < segment.count; j++)
            {
                SpawnEnemy(i);
                i++;
                yield return new WaitForSeconds(segment.delay);
            }

            if (i > wave.enemies.Count)
                break;
        }

        _spawnRoutine = null;
    }

    private void SpawnEnemy(int i)
    {
        if (i >= Enemies.Count)
            return;
        GameObject enemy = Instantiate(Enemies[i], this.transform.position, this.transform.rotation);

        _enemiesSpawned++;
        _enemiesAlive++;

        EnemyController controller =  enemy.GetComponent<EnemyController>();
        controller.startPath(path);
        controller.onDeath += OnEnemyDied;
    }

    private void StartRound()
    {
        if(_spawnRoutine == null)
        {
            _enemiesSpawned = 0;
            _enemiesAlive = 0;
            Enemies = (roundNum >= 0 && roundNum < waves.WaveCount && waves.GetWave(roundNum) != null)
                ? waves.GetWave(roundNum).enemies
                : Enemies;
            roundNum++;
            UIManager.instance.StatusBarContoller.StartRoundUI(roundNum);
            _spawnRoutine = StartCoroutine(SpawnEnemies());
        }

        PlayStartSound();
    }

    private void EndRound()
    {
        if (roundNum >= waves.WaveCount)
        {
            victory?.Invoke();
            return;
        }
        endOfRound?.Invoke();
    }

    private void PlayStartSound()
    {
        var enemy = Enemies[0];
        Sounds sound;
        switch (enemy.GetComponent<EnemyController>().Type)
        {
            case EnemyType.Skeleton:
                sound = Sounds.Bones;
                break;
            case EnemyType.Werewolf:
                sound = Sounds.FootstepsMarch;
                break;
            case EnemyType.FrostGolem:
                sound = Sounds.Ice;
                break;
            default:
                sound = Sounds.FootstepsMarch;
                break;
        }

        SoundManager.instance.PlaySFX(sound);
    }

    private void OnEnemyDied()
    {
        _enemiesAlive--;

        if (_enemiesAlive == 0 && _enemiesSpawned == Enemies.Count)
        {
            EndRound();
        }
    }
}
