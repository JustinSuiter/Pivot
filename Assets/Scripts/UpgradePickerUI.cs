using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradePickerUI : MonoBehaviour
{
    public static UpgradePickerUI Instance;

    [Header("Panel")]
    public GameObject upgradePanel;

    [Header("Button A")]
    public Button upgradeButtonA;
    public TextMeshProUGUI upgradeTextA;

    [Header("Button B")]
    public Button upgradeButtonB;
    public TextMeshProUGUI upgradeTextB;

    [Header("Skip")]
    public Button skipButton;

    [Header("All Upgrades")]
    public List<UpgradeData> allUpgrades;

    private UpgradeData _currentA;
    private UpgradeData _currentB;

    void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        Debug.Log("Text A object: " + (upgradeTextA == null ? "NULL" : upgradeTextA.name));
        Debug.Log("Text B object: " + (upgradeTextB == null ? "NULL" : upgradeTextB.name));
        Debug.Log("Upgrade A: " + (_currentA == null ? "NULL" : _currentA.upgradeName));
        Debug.Log("Upgrade B: " + (_currentB == null ? "NULL" : _currentB.upgradeName));
        Debug.Log("Showing upgrade picker. Pool size: " + allUpgrades.Count);

        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
        int indexA = Random.Range(0, pool.Count);
        _currentA = pool[indexA];
        pool.RemoveAt(indexA);
        _currentB = pool[Random.Range(0, pool.Count)];

        upgradeTextA.text = _currentA.upgradeName + "\n\n"
            + "<size=14><color=#AAAAAA>" + _currentA.description + "</color></size>";

        upgradeTextB.text = _currentB.upgradeName + "\n\n"
            + "<size=14><color=#AAAAAA>" + _currentB.description + "</color></size>";

        upgradePanel.SetActive(true);

        upgradeButtonA.onClick.RemoveAllListeners();
        upgradeButtonA.onClick.AddListener(() => SelectUpgrade(_currentA));

        upgradeButtonB.onClick.RemoveAllListeners();
        upgradeButtonB.onClick.AddListener(() => SelectUpgrade(_currentB));

        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(Hide);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SelectUpgrade(UpgradeData upgrade)
    {
        AudioManager.Instance?.PlayUpgradeSelect();
        Debug.Log("Selected upgrade: " + upgrade.upgradeName);
        UpgradeManager.Instance?.ApplyUpgrade(upgrade);
        Hide();
    }

    void Hide()
    {
        upgradePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        WaveClearManager.Instance?.AfterUpgradePicked();
    }
}