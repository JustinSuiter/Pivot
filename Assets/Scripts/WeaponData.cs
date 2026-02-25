using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "BackToBack/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponName;
    public string description;
    public Sprite icon;
    public int cost;

    [Header("Stats")]
    public float damage;
    public float swingCooldown;
    public float hitRadius;
    public float hitRange;
    public float swingAngle;

    [Header("Side")]
    public bool isFrontWeapon;

    [Header("Visuals")]
    public GameObject weaponMeshPrefab;
}