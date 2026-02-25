using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public Transform playerTransform;

    [Header("Wave Data")]
    public List<WaveData> waves;
    public WaveData escalationWave;

    [Header("Spawn Points")]
    public Transform spawnPointParent;
    private Transform[] _spawnPoints;

    [Header("State")]
    public int currentWaveIndex = 0;
    public int enemiesAlive = 0;
    public bool waveInProgress = false;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _spawnPoints = new Transform[spawnPointParent.childCount];
        for (int i = 0; i < spawnPointParent.childCount; i++)
            _spawnPoints[i] = spawnPointParent.GetChild(i);

        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        waveInProgress = false;

        WaveData data = currentWaveIndex < waves.Count
            ? waves[currentWaveIndex]
            : escalationWave;

        Debug.Log("Wave " + (currentWaveIndex + 1) + " starting in " + data.waveStartDelay + "s");
        yield return new WaitForSeconds(data.waveStartDelay);
        AudioManager.Instance?.PlayWaveStart();

        waveInProgress = true;
        GameManager.Instance?.OnWaveStart(currentWaveIndex + 1);

        foreach (WaveData.EnemySpawnInfo spawnInfo in data.enemies)
        {
            for (int i = 0; i < spawnInfo.count; i++)
            {
                SpawnEnemy(spawnInfo.enemyPrefab);
                yield return new WaitForSeconds(spawnInfo.spawnDelay);
            }
            yield return new WaitForSeconds(data.timeBetweenSpawns);
        }

        yield return new WaitUntil(() => enemiesAlive <= 0);

        int scoreGained = 500;
        GameManager.Instance?.OnWaveCleared(currentWaveIndex + 1);
        currentWaveIndex++;

        WaveClearManager.Instance?.ShowWaveClear(currentWaveIndex, scoreGained);
    }

    void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        EnemyBase eb = enemy.GetComponent<EnemyBase>();
        if (eb != null)
            eb.player = playerTransform;

        enemiesAlive++;
    }

    public void HandleEnemyDeath()
    {
        enemiesAlive--;
    }

    public void StartNextWaveFromManager()
    {
        StartCoroutine(StartNextWave());
    }
}