using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("References")]
    public DualCharacterController dualController;
    public CombatController combatController;
    public StaminaSystem staminaSystem;
    public ShieldController shieldController;

    private bool _lastStandUsed = false;
    private bool _lastStandAvailable = false;
    private float _frenzyMultiplier = 1f;
    private float _frenzyTimer = 0f;
    private int _frenzyStack = 0;
    private float _frenzyWindow = 2f;
    private bool _hasFrenzy = false;
    private float _damageReduction = 0f;

    public List<UpgradeData> activeUpgrades = new List<UpgradeData>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_hasFrenzy)
            UpdateFrenzy();
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        activeUpgrades.Add(upgrade);

        switch (upgrade.upgradeType)
        {
            case UpgradeData.UpgradeType.Reflexes:
                dualController.flipCooldown *= upgrade.value;
                break;

            case UpgradeData.UpgradeType.ThickSkin:
                _damageReduction += upgrade.value;
                break;

            case UpgradeData.UpgradeType.LastStand:
                _lastStandAvailable = true;
                break;

            case UpgradeData.UpgradeType.Frenzy:
                _hasFrenzy = true;
                break;

            case UpgradeData.UpgradeType.WiderArc:
                combatController.swingAngle += upgrade.value;
                break;

            case UpgradeData.UpgradeType.WeightedSwing:
                combatController.enableStagger = true;
                break;
        }

        Debug.Log("Upgrade applied: " + upgrade.upgradeName);
    }

    public void OnHitLanded()
    {
        if (!_hasFrenzy) return;

        _frenzyTimer = _frenzyWindow;
        _frenzyStack = Mathf.Min(_frenzyStack + 1, 5);
        _frenzyMultiplier = 1f + (_frenzyStack * 0.2f);
        combatController.damage = combatController.baseDamage * _frenzyMultiplier;
    }

    public void OnFlip()
    {
        if (!_hasFrenzy) return;
        ResetFrenzy();
    }

    void UpdateFrenzy()
    {
        if (_frenzyStack == 0) return;

        _frenzyTimer -= Time.deltaTime;
        if (_frenzyTimer <= 0f)
            ResetFrenzy();
    }

    void ResetFrenzy()
    {
        _frenzyStack = 0;
        _frenzyMultiplier = 1f;
        if (combatController != null)
            combatController.damage = combatController.baseDamage;
    }

    public float ProcessDamageReduction(float damage)
    {
        return damage * (1f - _damageReduction);
    }

    public bool TryLastStand()
    {
        if (_lastStandAvailable && !_lastStandUsed)
        {
            _lastStandUsed = true;
            _lastStandAvailable = false;
            Debug.Log("LAST STAND TRIGGERED");
            return true;
        }
        return false;
    }
}