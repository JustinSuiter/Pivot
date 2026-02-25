using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    public static FoodSpawner Instance;

    [Header("Food Settings")]
    public GameObject[] foodPrefabs;
    public float healAmount = 20f;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 8f;
    public float maxSpawnInterval = 20f;
    public int maxFoodOnArena = 2;
    public Transform spawnPointParent;

    private Transform[] _spawnPoints;
    private List<GameObject> _activeFood = new List<GameObject>();
    private bool _waveActive = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _spawnPoints = new Transform[spawnPointParent.childCount];
        for (int i = 0; i < spawnPointParent.childCount; i++)
            _spawnPoints[i] = spawnPointParent.GetChild(i);

        StartCoroutine(SpawnLoop());
    }

    public void SetWaveActive(bool active)
    {
        _waveActive = active;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (!_waveActive) continue;
            if (GameManager.Instance != null && GameManager.Instance.isGameOver) continue;

            _activeFood.RemoveAll(f => f == null);

            if (_activeFood.Count < maxFoodOnArena)
                SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (foodPrefabs == null || foodPrefabs.Length == 0) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        GameObject prefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];

        Vector3 spawnPos = spawnPoint.position + Vector3.up * 0.8f;
        GameObject food = Instantiate(prefab, spawnPos, Quaternion.identity);

        FoodPickup pickup = food.GetComponent<FoodPickup>();
        if (pickup != null)
            pickup.healAmount = healAmount;

        _activeFood.Add(food);
    }
}