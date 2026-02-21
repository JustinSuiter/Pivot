using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("Bars")]
    public Slider hpBar;
    public Slider staminaBar;
    public Slider syncBar;

    [Header("Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;

    [Header("Side Indicator")]
    public Image sideIndicator;
    public Color frontColor = new Color(1f, 0.27f, 0.27f);
    public Color backColor = new Color(0.27f, 0.53f, 1f);

    [Header("References")]
    public DualCharacterController dualController;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateBars();
        UpdateSideIndicator();
    }

    void UpdateBars()
    {
        if (GameManager.Instance != null)
        {
            hpBar.value = GameManager.Instance.playerHP;
            hpBar.maxValue = GameManager.Instance.maxHP;
        }

        if (StaminaSystem.Instance != null)
        {
            staminaBar.value = StaminaSystem.Instance.currentStamina;
            staminaBar.maxValue = StaminaSystem.Instance.maxStamina;
        }

        if (SyncMeter.Instance != null)
        {
            syncBar.value = SyncMeter.Instance.currentSync;
            syncBar.maxValue = SyncMeter.Instance.maxSync;
        }
    }

    void UpdateSideIndicator()
    {
        if (dualController == null) return;
        sideIndicator.color = dualController.isFacingFront
            ? frontColor
            : backColor;
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    public void UpdateWave(int wave)
    {
        if (waveText != null)
            waveText.text = "WAVE " + wave;
    }
}