using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyBaseController : MonoBehaviour
{
    [Header("Path")]
    public SplineContainer path;

    [Header("Enemies")]
    public List<GameObject> Enemies;
    public float defaultSpawnDelay = 0.3f;

    public int roundNum = 0;

    private Coroutine _spawnRoutine = null;
    private int _enemiesSpawned = 0;
    private int _enemiesAlive = 0;

    private void OnEnable()
    {
        UIManager.nextRoundClicked += StartRound;
    }
    private void OnDisable()
    {
        UIManager.nextRoundClicked -= StartRound;
    }

    private System.Collections.IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            SpawnEnemy(i);
            yield return new WaitForSeconds(defaultSpawnDelay);
        }

        _spawnRoutine = null;
    }

    private void SpawnEnemy(int i)
    {
        GameObject enemy = Instantiate(Enemies[i]);

        _enemiesSpawned++;
        _enemiesAlive++;

        //Assign to path
        EnemyController controller =  enemy.GetComponent<EnemyController>();
        controller.startPath(path);
        controller.onDeath += OnEnemyDied;
    }

    //Start the round if there is not already a round to start
    private void StartRound()
    {
        if(_spawnRoutine == null)
        {
            _enemiesSpawned = 0;
            _enemiesAlive = 0;
            roundNum++;
            UIManager.instance.StartRoundUI(roundNum);
            _spawnRoutine = StartCoroutine(SpawnEnemies());
        }
    }

    private void EndRound()
    {
        UIManager.instance.EndRoundUI();
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
