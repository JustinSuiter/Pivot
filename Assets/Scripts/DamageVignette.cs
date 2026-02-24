using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    public static DamageVignette Instance;

    public Image vignetteImage;
    public float flashAlpha = 0.5f;
    public float fadeSpeed = 4f;

    private float _currentAlpha = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_currentAlpha > 0f)
        {
            _currentAlpha -= Time.deltaTime * fadeSpeed;
            _currentAlpha = Mathf.Max(0f, _currentAlpha);
            vignetteImage.color = new Color(1f, 0f, 0f, _currentAlpha);
        }

        if (GameManager.Instance != null)
        {
            float hpPercent = GameManager.Instance.playerHP / GameManager.Instance.maxHP;
            if (hpPercent < 0.3f && _currentAlpha < 0.1f)
            {
                float pulse = (Mathf.Sin(Time.time * 3f) + 1f) / 2f;
                vignetteImage.color = new Color(1f, 0f, 0f, pulse * 0.25f);
            }
        }
    }

    public void Flash()
    {
        _currentAlpha = flashAlpha;
    }

    public void FlashBig()
    {
        _currentAlpha = 0.8f;
    }
}