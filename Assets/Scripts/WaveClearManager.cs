using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaveClearManager : MonoBehaviour
{
    public static WaveClearManager Instance;

    [Header("Panel")]
    public GameObject waveClearPanel;
    public TextMeshProUGUI waveClearSubtitle;
    public TextMeshProUGUI waveClearScore;
    public TextMeshProUGUI countdownText;
    public GameObject marketButton;

    [Header("Settings")]
    public float displayTime = 3f;
    public int marketEveryNWaves = 3;

    private bool _marketPending = false;

    void Awake()
    {
        Instance = this;
    }

    public void ShowWaveClear(int waveNumber, int scoreGained)
    {
        _marketPending = waveNumber % marketEveryNWaves == 0;
        StartCoroutine(WaveClearSequence(waveNumber, scoreGained));
    }

    IEnumerator WaveClearSequence(int waveNumber, int scoreGained)
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;

        waveClearPanel.SetActive(true);
        waveClearSubtitle.text = "WAVE " + waveNumber + " COMPLETE";
        waveClearScore.text = "+" + scoreGained;

        if (_marketPending)
        {
            marketButton.SetActive(true);
            countdownText.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            yield break;
        }
        else
        {
            marketButton.SetActive(false);
            countdownText.gameObject.SetActive(true);

            float countdown = displayTime;
            while (countdown > 0f)
            {
                countdownText.text = "NEXT WAVE IN " + Mathf.CeilToInt(countdown);
                countdown -= Time.deltaTime;
                yield return null;
            }

            HideWaveClear();
        }
    }

    public void HideWaveClear()
    {
        waveClearPanel.SetActive(false);
        marketButton.SetActive(false);
        countdownText.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        WaveManager.Instance?.StartNextWaveFromManager();
    }
}