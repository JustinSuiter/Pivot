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

    [Header("Sync")]
    public GameObject syncReadyText;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateBars();
        UpdateSideIndicator();
        if (StaminaSystem.Instance != null && StaminaSystem.Instance.IsExhausted())
            {
                float pulse = (Mathf.Sin(Time.time * 8f) + 1f) / 2f;
                ColorBlock cb = staminaBar.colors;
                staminaBar.GetComponentInChildren<Image>().color =
                    Color.Lerp(new Color(1f, 0.4f, 0f), Color.red, pulse);
            }
    }

    void UpdateBars()
    {
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

    public void UpdateSyncReady(bool ready)
    {
        if (syncReadyText != null)
            syncReadyText.SetActive(ready);
    }

    public void UpdateHP(float current, float max)
    {
        if (hpBar != null)
        {
            hpBar.value = current;
            hpBar.maxValue = max;
        }
    }
}