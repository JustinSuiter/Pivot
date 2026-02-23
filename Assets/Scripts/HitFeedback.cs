using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitFeedback : MonoBehaviour
{
    public static HitFeedback Instance;

    [Header("Screen Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    [Header("Freeze Frame")]
    public int freezeFrames = 3;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip swingSound;

    [Header("Visuals")]
    public Image syncBurstFlash;

    private AudioSource _audioSource;
    private Vector3 _cameraOriginalPos;
    private Transform _activeCameraTransform;

    void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetActiveCamera(Transform camTransform)
    {
        _activeCameraTransform = camTransform;
        _cameraOriginalPos = camTransform.localPosition;
    }

    public void OnSwing()
    {
        if (swingSound != null)
            _audioSource.PlayOneShot(swingSound, 0.6f);
    }

    public void OnHit()
    {
        if (hitSound != null)
            _audioSource.PlayOneShot(hitSound, 1f);

        StartCoroutine(FreezeFrame());
        StartCoroutine(Shake());
    }

    IEnumerator FreezeFrame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeFrames / 60f);
        Time.timeScale = 1f;
    }

    IEnumerator Shake()
    {
        if (_activeCameraTransform == null) yield break;

        _cameraOriginalPos = _activeCameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            _activeCameraTransform.localPosition = _cameraOriginalPos + new Vector3(x, y, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _activeCameraTransform.localPosition = _cameraOriginalPos;
    }

    public void TriggerSyncBurst()
    {
        StartCoroutine(BurstEffect());
    }

    IEnumerator BurstEffect()
    {
        // Flash screen cyan
        if (syncBurstFlash != null)
            syncBurstFlash.color = new Color(0.27f, 1f, 0.67f, 0.6f);

        // Big freeze frame
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;

        // Fade flash out
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.unscaledDeltaTime;
            if (syncBurstFlash != null)
            {
                float alpha = Mathf.Lerp(0.6f, 0f, elapsed / 0.3f);
                syncBurstFlash.color = new Color(0.27f, 1f, 0.67f, alpha);
            }
            yield return null;
        }

        if (syncBurstFlash != null)
            syncBurstFlash.color = Color.clear;

        // Screen shake
        if (_activeCameraTransform != null)
        {
            float shakeElapsed = 0f;
            float duration = 0.3f;
            float magnitude = 0.15f;

            while (shakeElapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                _activeCameraTransform.localPosition = _cameraOriginalPos + new Vector3(x, y, 0f);
                shakeElapsed += Time.unscaledDeltaTime;
                magnitude *= 0.95f;
                yield return null;
            }

            _activeCameraTransform.localPosition = _cameraOriginalPos;
        }
    }
}