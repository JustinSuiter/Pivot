using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour
{
    [Header("References")]
    public DualCharacterController dualController;
    public Transform weaponPivot_Front;
    public Transform weaponPivot_Back;

    [Header("Combat Stats")]
    public float damage = 25f;
    [HideInInspector] public float baseDamage;
    public float swingCooldown = 0.35f;
    public float hitRadius = 1.2f;
    public float hitRange = 1.5f;
    public float swingAngle = 60f;
    public bool enableStagger = false;
    public LayerMask enemyLayer;

    [Header("Front Slash Settings")]
    public float slashDuration = 0.18f;
    public float slashSwipeDistance = 0.5f;
    public float slashPeakScale = 1.15f;

    [Header("Back Bash Settings")]
    public float bashDuration = 0.15f;
    public float bashForwardDistance = 0.4f;
    public float bashPeakScale = 1.1f;

    private float _swingTimer;
    private bool _isSwinging;

    private Vector3 _frontOriginalPos;
    private Quaternion _frontOriginalRot;
    private Vector3 _frontOriginalScale;

    private Vector3 _backOriginalPos;
    private Quaternion _backOriginalRot;
    private Vector3 _backOriginalScale;

    void Start()
    {
        baseDamage = damage;

        if (weaponPivot_Front != null)
        {
            _frontOriginalPos = weaponPivot_Front.localPosition;
            _frontOriginalRot = weaponPivot_Front.localRotation;
            _frontOriginalScale = weaponPivot_Front.localScale;
        }

        if (weaponPivot_Back != null)
        {
            _backOriginalPos = weaponPivot_Back.localPosition;
            _backOriginalRot = weaponPivot_Back.localRotation;
            _backOriginalScale = weaponPivot_Back.localScale;
        }
    }

    void Update()
    {
        _swingTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _swingTimer <= 0f && !_isSwinging)
        {
            if (dualController.isFacingFront)
                StartCoroutine(PerformSlash());
            else
                StartCoroutine(PerformBash());
        }
    }

    IEnumerator PerformSlash()
    {
        _isSwinging = true;
        _swingTimer = swingCooldown;

        AudioManager.Instance?.PlaySwing();

        Vector3 startPos = _frontOriginalPos + new Vector3(0.3f, 0f, 0f);
        Quaternion startRot = _frontOriginalRot * Quaternion.Euler(0f, 0f, -40f);
        Vector3 startScale = _frontOriginalScale;

        Vector3 endPos = _frontOriginalPos + new Vector3(-slashSwipeDistance, 0f, 0f);
        Quaternion endRot = _frontOriginalRot * Quaternion.Euler(0f, 0f, 40f);
        Vector3 peakScale = _frontOriginalScale * slashPeakScale;

        weaponPivot_Front.localPosition = startPos;
        weaponPivot_Front.localRotation = startRot;

        bool hitChecked = false;
        float elapsed = 0f;

        while (elapsed < slashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slashDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            weaponPivot_Front.localPosition = Vector3.Lerp(startPos, endPos, smoothT);
            weaponPivot_Front.localRotation = Quaternion.Lerp(startRot, endRot, smoothT);

            float scaleT = Mathf.Sin(t * Mathf.PI);
            weaponPivot_Front.localScale = Vector3.Lerp(startScale, peakScale, scaleT);

            if (!hitChecked && t >= 0.5f)
            {
                hitChecked = true;
                CheckHit(weaponPivot_Front);
            }

            yield return null;
        }

        yield return StartCoroutine(ReturnToOriginal(
            weaponPivot_Front,
            _frontOriginalPos,
            _frontOriginalRot,
            _frontOriginalScale
        ));

        _isSwinging = false;
    }

    IEnumerator PerformBash()
    {
        _isSwinging = true;
        _swingTimer = swingCooldown;

        AudioManager.Instance?.PlaySwing();

        Vector3 startPos = _backOriginalPos;
        Quaternion startRot = _backOriginalRot;
        Vector3 startScale = _backOriginalScale;

        Vector3 bashPos = _backOriginalPos + new Vector3(0f, 0f, bashForwardDistance);
        Vector3 peakScale = _backOriginalScale * bashPeakScale;

        bool hitChecked = false;
        float elapsed = 0f;

        while (elapsed < bashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bashDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            weaponPivot_Back.localPosition = Vector3.Lerp(startPos, bashPos, smoothT);

            float scaleT = Mathf.Sin(t * Mathf.PI);
            weaponPivot_Back.localScale = Vector3.Lerp(startScale, peakScale, scaleT);

            if (!hitChecked && t >= 0.6f)
            {
                hitChecked = true;
                CheckHit(weaponPivot_Back);
            }

            yield return null;
        }

        yield return StartCoroutine(ReturnToOriginal(
            weaponPivot_Back,
            _backOriginalPos,
            _backOriginalRot,
            _backOriginalScale
        ));

        _isSwinging = false;
    }

    IEnumerator ReturnToOriginal(Transform pivot, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        float elapsed = 0f;
        float returnDuration = 0.1f;

        Vector3 currentPos = pivot.localPosition;
        Quaternion currentRot = pivot.localRotation;
        Vector3 currentScale = pivot.localScale;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            pivot.localPosition = Vector3.Lerp(currentPos, pos, t);
            pivot.localRotation = Quaternion.Lerp(currentRot, rot, t);
            pivot.localScale = Vector3.Lerp(currentScale, scale, t);

            yield return null;
        }

        pivot.localPosition = pos;
        pivot.localRotation = rot;
        pivot.localScale = scale;
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
                HitFeedback.Instance?.SetActiveCamera(dualController.GetActiveCamera().transform);
                HitFeedback.Instance?.OnHit();
                AudioManager.Instance?.PlayHit();
                AudioManager.Instance?.PlayEnemyHit();
                SyncMeter.Instance?.ArmFromHit();
                UpgradeManager.Instance?.OnHitLanded();

                if (enableStagger)
                    enemy.ApplyStagger(0.5f);
            }
        }
    }

    public void CheckHitWithMultiplier(Transform pivot, float multiplier)
    {
        CheckHit(pivot, multiplier);
    }
}