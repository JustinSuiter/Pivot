using UnityEngine;
using System.Collections;

public class FoodPickup : MonoBehaviour
{
    [Header("Settings")]
    public float healAmount = 20f;
    public float collectRadius = 1.5f;
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;
    public float lifetime = 15f;

    private Transform _player;
    private Vector3 _startPos;
    private float _lifetimeTimer;

    void Start()
    {
        _startPos = transform.position;
        _lifetimeTimer = lifetime;

        CharacterController cc = FindFirstObjectByType<CharacterController>();
        if (cc != null) _player = cc.transform;

        StartCoroutine(FadeOutBeforeDestroy());
    }

    void Update()
    {
        if (_player == null) return;

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        float newY = _startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= collectRadius)
            Collect();

        _lifetimeTimer -= Time.deltaTime;
        if (_lifetimeTimer <= 0f)
            Destroy(gameObject);
    }

    void Collect()
    {
        GameManager.Instance?.HealPlayer(healAmount);
        Destroy(gameObject);
    }

    IEnumerator FadeOutBeforeDestroy()
    {
        yield return new WaitForSeconds(lifetime - 3f);

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null) yield break;

        float elapsed = 0f;
        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.Sin(elapsed * 10f);
            rend.enabled = pulse > 0f;
            yield return null;
        }
    }
}