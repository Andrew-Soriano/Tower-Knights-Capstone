using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public List<GameObject> enemies = new List<GameObject>();
    public List<SpawnSegment> segments = new List<SpawnSegment>();

    public EnemyWave(List<GameObject> enemies, List<SpawnSegment> segments)
    {
        this.enemies = enemies;
        this.segments = segments;
    }
}

[Serializable]
public class SpawnSegment
{
    public int count;
    public float delay;

    public SpawnSegment(int count, float delay)
    {
        this.count = count;
        this.delay = delay;
    }
}

[CreateAssetMenu(fileName = "EnemyWaves", menuName = "Scriptable Objects/EnemyWaves")]
public class EnemyWaves : ScriptableObject
{
    [SerializeField] private List<EnemyWave> waves = new List<EnemyWave>();
    public int WaveCount { get => waves.Count; }

    public EnemyWave GetWave(int waveId)
    {
        if (waveId < 0 || waveId >= waves.Count)
        {
            Debug.LogWarning($"Wave ID {waveId} is out of range. Returning null.");
            return null;
        }

        return waves[waveId];
    }
}
