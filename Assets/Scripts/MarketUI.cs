using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MarketUI : MonoBehaviour
{
    public static MarketUI Instance;

    [Header("Panel")]
    public GameObject marketPanel;
    public TextMeshProUGUI marketScoreText;

    [Header("Weapon Cards")]
    public Button[] weaponButtons;
    public TextMeshProUGUI[] weaponNames;
    public TextMeshProUGUI[] weaponDescs;
    public TextMeshProUGUI[] weaponSides;
    public TextMeshProUGUI[] weaponCosts;

    [Header("Close")]
    public Button closeButton;

    [Header("All Weapons")]
    public List<WeaponData> allWeapons;

    private List<WeaponData> _displayedWeapons = new List<WeaponData>();

    void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        marketPanel.SetActive(true);
        RefreshScore();
        PopulateWeapons();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void PopulateWeapons()
    {
        List<WeaponData> pool = new List<WeaponData>(allWeapons);
        _displayedWeapons.Clear();

        for (int i = 0; i < weaponButtons.Length && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            WeaponData weapon = pool[idx];
            pool.RemoveAt(idx);
            _displayedWeapons.Add(weapon);

            weaponNames[i].text = weapon.weaponName;
            weaponDescs[i].text = weapon.description;
            weaponSides[i].text = weapon.isFrontWeapon ? "FRONT" : "BACK";
            weaponCosts[i].text = weapon.cost + " PTS";

            int index = i;
            weaponButtons[i].onClick.RemoveAllListeners();
            weaponButtons[i].onClick.AddListener(() => TryBuyWeapon(index));

            ColorBlock cb = weaponButtons[i].colors;
            cb.normalColor = GameManager.Instance.score >= weapon.cost
                ? new Color(0.1f, 0.1f, 0.18f)
                : new Color(0.1f, 0.05f, 0.05f);
            weaponButtons[i].colors = cb;
        }
    }

    void TryBuyWeapon(int index)
    {
        if (index >= _displayedWeapons.Count) return;
        WeaponData weapon = _displayedWeapons[index];

        if (GameManager.Instance.score < weapon.cost)
        {
            AudioManager.Instance?.PlayMarketBuy();
            Debug.Log("Not enough points for " + weapon.weaponName);
            return;
        }

        GameManager.Instance.SpendScore(weapon.cost);
        WeaponEquipper.Instance?.EquipWeapon(weapon);
        RefreshScore();
        PopulateWeapons();
    }

    void RefreshScore()
    {
        marketScoreText.text = "SCORE: " + GameManager.Instance.score;
    }

    void Hide()
    {
        marketPanel.SetActive(false);
        WaveClearManager.Instance?.ResumeGame();
    }
}