using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public float playerHP = 150f;
    public float maxHP = 150f;

    [Header("Score")]
    public int score = 0;
    private int _highScore = 0;

    [Header("State")]
    public bool isGameOver = false;
    public bool isGamePaused = false;

    [Header("References")]
    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI finalScoreText;
    public TMPro.TextMeshProUGUI highScoreText;

    void Awake()
    {
        Instance = this;
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        HUDManager.Instance?.UpdateScore(score);
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver) return;

        float afterShield = ShieldController.Instance != null
            ? ShieldController.Instance.ProcessIncomingDamage(amount)
            : amount;

        float actualDamage = UpgradeManager.Instance != null
            ? UpgradeManager.Instance.ProcessDamageReduction(afterShield)
            : afterShield;

        playerHP -= actualDamage;
        playerHP = Mathf.Max(0f, playerHP);

        if (actualDamage >= 10f)
            DamageVignette.Instance?.Flash();

        HUDManager.Instance?.UpdateHP(playerHP, maxHP);

        if (playerHP <= 0f)
        {
            if (UpgradeManager.Instance != null && UpgradeManager.Instance.TryLastStand())
            {
                playerHP = 1f;
                HUDManager.Instance?.UpdateHP(playerHP, maxHP);
                DamageVignette.Instance?.FlashBig();
                return;
            }
            TriggerGameOver();
        }
    }

    public void OnWaveStart(int waveNumber)
    {
        HUDManager.Instance?.UpdateWave(waveNumber);
    }

    public void OnWaveCleared(int waveNumber)
    {
        AddScore(500);
        HUDManager.Instance?.UpdateWave(waveNumber + 1);
    }

    void TriggerGameOver()
    {
        AudioManager.Instance?.PlayDeath();
        if (isGameOver) return;
        isGameOver = true;

        if (score > _highScore)
        {
            _highScore = score;
            PlayerPrefs.SetInt("HighScore", _highScore);
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "SCORE: " + score;

            if (highScoreText != null)
                highScoreText.text = "BEST: " + _highScore;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DamageVignette.Instance?.FlashBig();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SpendScore(int amount)
    {
        score -= amount;
        score = Mathf.Max(0, score);
        HUDManager.Instance?.UpdateScore(score);
    }
}