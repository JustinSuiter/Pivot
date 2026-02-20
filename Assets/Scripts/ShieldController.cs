using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    public static ShieldController Instance;

    [Header("References")]
    public DualCharacterController dualController;
    public GameObject shieldObject;

    [Header("Parry Settings")]
    public float parryWindow = 0.3f;
    public float perfectParryDamageNegate = 1f;
    public float normalBlockDamageReduction = 0.8f;

    [Header("Sync Reward")]
    public float parrysynsReward = 25f;

    private bool _isBlocking;
    private bool _inParryWindow;
    private float _parryTimer;

    public bool IsBlocking => _isBlocking;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (dualController.isFacingFront)
        {
            StopBlocking();
            return;
        }

        if (Input.GetMouseButtonDown(1))
            StartBlocking();

        if (Input.GetMouseButtonUp(1))
            StopBlocking();

        if (_inParryWindow)
        {
            _parryTimer -= Time.deltaTime;
            if (_parryTimer <= 0f)
                _inParryWindow = false;
        }
    }

    void StartBlocking()
    {
        if (StaminaSystem.Instance.IsExhausted()) return;

        _isBlocking = true;
        _inParryWindow = true;
        _parryTimer = parryWindow;

        if (shieldObject != null)
            shieldObject.SetActive(true);
    }

    void StopBlocking()
    {
        _isBlocking = false;
        _inParryWindow = false;

        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    public float ProcessIncomingDamage(float rawDamage)
    {
        if (!_isBlocking)
            return rawDamage;

        float damageMultiplier = StaminaSystem.Instance.UseBlockStamina();

        if (_inParryWindow)
        {
            SyncMeter.Instance?.AddSync(parrysynsReward);
            HitFeedback.Instance?.OnHit();
            Debug.Log("Perfect Parry!");
            return 0f;
        }

        return rawDamage * (1f - normalBlockDamageReduction) * damageMultiplier;
    }
}