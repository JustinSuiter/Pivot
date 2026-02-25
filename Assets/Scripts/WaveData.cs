using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveData", menuName = "BackToBack/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay;
    }

    public List<EnemySpawnInfo> enemies;
    public float timeBetweenSpawns = 0.8f;
    public float waveStartDelay = 2f;
}