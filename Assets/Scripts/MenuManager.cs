using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject howToPlayPanel;

    [Header("Canvas Groups")]
    public CanvasGroup settingsCG;
    public CanvasGroup howToPlayCG;

    [Header("Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider fovSlider;
    public AudioSource musicSource;

    [Header("UI")]
    public TextMeshProUGUI highScoreText;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Audio")]
    public AudioClip clickSound;
    private AudioSource _sfxSource;

    void Start()
    {
        _sfxSource = gameObject.AddComponent<AudioSource>();

        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "BEST: " + hs;

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float savedFOV = PlayerPrefs.GetFloat("FOV", 80f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
        if (fovSlider != null)
        {
            fovSlider.minValue = 60f;
            fovSlider.maxValue = 110f;
            fovSlider.value = savedFOV;
            fovSlider.onValueChanged.AddListener(OnFOVChanged);
        }

        if (musicSource != null)
            musicSource.volume = savedMusic;

        settingsCG.alpha = 0f;
        settingsCG.interactable = false;
        settingsCG.blocksRaycasts = false;

        howToPlayCG.alpha = 0f;
        howToPlayCG.interactable = false;
        howToPlayCG.blocksRaycasts = false;
    }

    public void OnPlayPressed()
    {
        PlayClick();
        StartCoroutine(LoadGameScene());
    }

    public void OnSettingsPressed()
    {
        PlayClick();
        StartCoroutine(FadeIn(settingsCG));
    }

    public void OnHowToPlayPressed()
    {
        PlayClick();
        StartCoroutine(FadeIn(howToPlayCG));
    }

    public void OnExitPressed()
    {
        PlayClick();
        Application.Quit();
    }

    public void OnSettingsBack()
    {
        PlayClick();
        StartCoroutine(FadeOut(settingsCG));
    }

    public void OnHowToPlayBack()
    {
        PlayClick();
        StartCoroutine(FadeOut(howToPlayCG));
    }

    void OnMusicChanged(float value)
    {
        if (musicSource != null) musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    void OnFOVChanged(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
        PlayerPrefs.Save();
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("MainScene");
    }

    void PlayClick()
    {
        if (clickSound != null && _sfxSource != null)
            _sfxSource.PlayOneShot(clickSound);
    }
}