using UnityEngine;

public class WeaponEquipper : MonoBehaviour
{
    public static WeaponEquipper Instance;

    [Header("References")]
    public CombatController combatController;
    public Transform weaponPivotFront;
    public Transform weaponPivotBack;

    void Awake()
    {
        Instance = this;
    }

    public void EquipWeapon(WeaponData data)
    {
        Transform targetPivot = data.isFrontWeapon
            ? weaponPivotFront
            : weaponPivotBack;

        foreach (Transform child in targetPivot)
            Destroy(child.gameObject);

        if (data.weaponMeshPrefab != null)
            Instantiate(data.weaponMeshPrefab, targetPivot);

        if (data.isFrontWeapon)
        {
            combatController.damage = data.damage;
            combatController.baseDamage = data.damage;
            combatController.swingCooldown = data.swingCooldown;
            combatController.hitRadius = data.hitRadius;
            combatController.hitRange = data.hitRange;
            combatController.swingAngle = data.swingAngle;
        }
        else
        {
            ShieldController.Instance.backWeaponDamage = data.damage;
            ShieldController.Instance.parryWindow = data.cost == 700
                ? 0.5f
                : 0.3f;
        }

        Debug.Log("Equipped: " + data.weaponName);
    }
}