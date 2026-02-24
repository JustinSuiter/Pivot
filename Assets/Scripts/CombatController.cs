using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    [Header("References")]
    public DualCharacterController dualController;
    public Transform weaponPivot_Front;
    public Transform weaponPivot_Back;
    public Animator frontWeaponAnimator;
    private HitFeedback _hitFeedback;

    [Header("Combat Stats")]
    public float damage = 25f;
    public float swingCooldown = 0.4f;
    public float hitRadius = 1.2f;
    public float hitRange = 1.5f;
    public LayerMask enemyLayer;

    [Header("Swing Settings")]
    public float swingAngle = 60f;
    public float swingDuration = 0.15f;

    private float _swingTimer;
    private bool _isSinging;

    [HideInInspector] public float baseDamage;
    public bool enableStagger = false;

    void Start()
    {
        _hitFeedback = HitFeedback.Instance;
        baseDamage = damage;
    }
    void Update()
    {
        _swingTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _swingTimer <= 0f && !_isSinging)
        {
            StartCoroutine(PerformSwing());
        }
    }

    IEnumerator PerformSwing()
    {
        _isSinging = true;
        AudioManager.Instance?.PlaySwing();

        _swingTimer = swingCooldown;

        Transform activePivot = dualController.isFacingFront
            ? weaponPivot_Front
            : weaponPivot_Back;

        Quaternion startRot = activePivot.localRotation;
        Quaternion swingRot = startRot * Quaternion.Euler(-swingAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            activePivot.localRotation = Quaternion.Lerp(startRot, swingRot, elapsed / swingDuration);
            yield return null;
        }

        float damageMultiplier = StaminaSystem.Instance != null
            ? StaminaSystem.Instance.UseSwingStamina()
            : 1f;

        CheckHit(activePivot, damageMultiplier);
        elapsed = 0f;
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            activePivot.localRotation = Quaternion.Lerp(swingRot, startRot, elapsed / swingDuration);
            yield return null;
        }

        activePivot.localRotation = startRot;
        _isSinging = false;
    }

    void CheckHit(Transform pivot, float damageMultiplier = 1f)
    {
        Vector3 hitOrigin = pivot.position + pivot.forward * hitRange;
        Collider[] hits = Physics.OverlapSphere(hitOrigin, hitRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage * damageMultiplier);
                AudioManager.Instance?.PlayHit();
                AudioManager.Instance?.PlayEnemyHit();
                UpgradeManager.Instance?.OnHitLanded();

                if (enableStagger)
                    enemy.ApplyStagger(0.5f);
                SyncMeter.Instance?.ArmFromHit();
                HitFeedback.Instance?.SetActiveCamera(dualController.GetActiveCamera().transform);
                HitFeedback.Instance?.OnHit();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (weaponPivot_Front != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                weaponPivot_Front.position + weaponPivot_Front.forward * hitRange,
                hitRadius
            );
        }
    }
}