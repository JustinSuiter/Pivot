using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "BackToBack/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public enum UpgradeType
    {
        Reflexes,
        ThickSkin,
        LastStand,
        Frenzy,
        WiderArc,
        WeightedSwing
    }

    [Header("Identity")]
    public string upgradeName;
    public string description;
    public Sprite icon;
    public UpgradeType upgradeType;

    [Header("Value")]
    public float value;
}