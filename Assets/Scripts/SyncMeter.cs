using UnityEngine;
using System.Collections;

public class SyncMeter : MonoBehaviour
{
    public static SyncMeter Instance;

    [Header("Settings")]
    public float maxSync = 100f;
    public float currentSync = 0f;
    public float syncPerKill = 15f;
    public float syncPerHit = 5f;
    public float syncPerParry = 25f;

    [Header("Burst Settings")]
    public float burstDamage = 60f;
    public float burstRadius = 8f;
    public float burstDuration = 0.3f;
    public LayerMask enemyLayer;

    [Header("References")]
    public DualCharacterController dualController;

    [Header("Combo Window")]
    public float comboWindow = 2f;

    private float _hitArmedTimer = 0f;
    private float _blockArmedTimer = 0f;
    private bool _burstReady = false;
    private bool _isBursting = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        _hitArmedTimer -= Time.deltaTime;
        _blockArmedTimer -= Time.deltaTime;

        if (currentSync >= maxSync && !_burstReady && !_isBursting)
        {
            _burstReady = true;
            Debug.Log("SYNC BURST READY");
        }

        if (_burstReady && Input.GetKeyDown(KeyCode.F))
            StartCoroutine(TriggerBurst());
    }

    // Hit then block (or block then hit) within comboWindow fills meter
    public void ArmFromHit()
    {
        if (_blockArmedTimer > 0f)
        {
            AddSync(syncPerHit);
            _blockArmedTimer = 0f;
        }
        else
        {
            _hitArmedTimer = comboWindow;
        }
    }

    public void ArmFromBlock(bool isPerfectParry)
    {
        if (_hitArmedTimer > 0f)
        {
            AddSync(isPerfectParry ? syncPerParry : syncPerHit);
            _hitArmedTimer = 0f;
        }
        else
        {
            _blockArmedTimer = comboWindow;
        }
    }

    // Kills always contribute directly
    public void AddSyncOnKill() => AddSync(syncPerKill);

    private void AddSync(float amount)
    {
        if (_isBursting) return;
        currentSync = Mathf.Min(currentSync + amount, maxSync);
        HUDManager.Instance?.UpdateSyncReady(_burstReady);
    }

    IEnumerator TriggerBurst()
    {
        _isBursting = true;
        _burstReady = false;
        currentSync = 0f;

        Debug.Log("SYNC BURST ACTIVATED");
        HitFeedback.Instance?.TriggerSyncBurst();

        Collider[] hits = Physics.OverlapSphere(
            dualController.transform.position,
            burstRadius,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
                enemy.TakeDamage(burstDamage);
        }

        yield return new WaitForSeconds(burstDuration);
        _isBursting = false;

        HUDManager.Instance?.UpdateSyncReady(false);
    }
}