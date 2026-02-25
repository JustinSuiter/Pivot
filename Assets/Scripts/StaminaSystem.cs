using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenRate = 15f;
    public float regenDelay = 1.5f;
    public float swingCost = 20f;
    public float blockCost = 3f;
    public float exhaustedDamageMultiplier = 0.5f;
    public float exhaustedBlockMultiplier = 0.3f;

    private float _regenTimer;
    private bool _isExhausted;

    void Awake()
    {
        Instance = this;
        currentStamina = maxStamina;
    }

    void Update()
    {
        _regenTimer -= Time.deltaTime;
        if (_regenTimer <= 0f && currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        if (currentStamina <= 0f && !_isExhausted)
        {
            _isExhausted = true;
            Debug.Log("Exhausted");
        }
        else if (currentStamina > 20f && _isExhausted)
        {
            _isExhausted = false;
        }
    }


    public float UseSwingStamina()
    {
        _regenTimer = regenDelay;
        currentStamina -= swingCost;
        currentStamina = Mathf.Max(0f, currentStamina);
        return _isExhausted ? exhaustedDamageMultiplier : 1f;
    }


    public float UseBlockStamina()
    {
        _regenTimer = regenDelay;
        currentStamina -= blockCost;
        currentStamina = Mathf.Max(0f, currentStamina);
        return _isExhausted ? exhaustedBlockMultiplier : 0.1f;
    }

    public bool IsExhausted() => _isExhausted;
}