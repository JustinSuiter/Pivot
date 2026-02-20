using UnityEngine;

public class SyncMeter : MonoBehaviour
{
    public static SyncMeter Instance;

    public float maxSync = 100f;
    public float currentSync = 0f;

    void Awake()
    {
        Instance = this;
    }

    public void AddSync(float amount)
    {
        currentSync = Mathf.Min(currentSync + amount, maxSync);
        Debug.Log("Sync: " + currentSync);
    }
}