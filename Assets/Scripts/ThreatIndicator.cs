using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ThreatIndicator : MonoBehaviour
{
    [Header("References")]
    public DualCharacterController dualController;
    public Canvas hudCanvas;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;
    public float edgePadding = 60f;
    public Color arrowColor = new Color(1f, 0.2f, 0.2f, 0.85f);
    public float pulseSpeed = 3f;

    [Header("Arrow Prefab")]
    public GameObject arrowPrefab;

    private List<GameObject> _activeArrows = new List<GameObject>();
    private List<GameObject> _arrowPool = new List<GameObject>();
    private RectTransform _canvasRect;

    void Start()
    {
        _canvasRect = hudCanvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        ReturnAllArrows();
        ShowThreats();
    }

    void ShowThreats()
    {
        Collider[] nearby = Physics.OverlapSphere(
            dualController.transform.position,
            detectionRadius,
            enemyLayer
        );

        foreach (Collider col in nearby)
        {
            Vector3 blindDirection = dualController.isFacingFront
                ? -dualController.transform.forward
                : dualController.transform.forward;

            Vector3 dirToEnemy = (col.transform.position
                - dualController.transform.position).normalized;

            if (Vector3.Dot(dirToEnemy, blindDirection) > 0.3f)
                PlaceArrow(col.transform.position);
        }
    }

    void PlaceArrow(Vector3 enemyWorldPos)
    {
        Camera activeCam = dualController.GetActiveCamera();
        Vector3 screenPos = activeCam.WorldToViewportPoint(enemyWorldPos);

        if (screenPos.z < 0f)
        {
            screenPos.x = 1f - screenPos.x;
            screenPos.y = 1f - screenPos.y;
            screenPos.z = 0f;
        }

        float screenW = Screen.width;
        float screenH = Screen.height;

        Vector2 canvasPos = new Vector2(
            (screenPos.x - 0.5f) * screenW,
            (screenPos.y - 0.5f) * screenH
        );

        float halfW = (screenW / 2f) - edgePadding;
        float halfH = (screenH / 2f) - edgePadding;

        if (Mathf.Abs(canvasPos.x) <= halfW && Mathf.Abs(canvasPos.y) <= halfH)
            return;

        float scale = Mathf.Min(halfW / Mathf.Abs(canvasPos.x),
                                halfH / Mathf.Abs(canvasPos.y));
        Vector2 clampedPos = canvasPos * scale;

        GameObject arrow = GetArrow();
        RectTransform rt = arrow.GetComponent<RectTransform>();
        rt.anchoredPosition = clampedPos;

        float angle = Mathf.Atan2(canvasPos.y, canvasPos.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        Image img = arrow.GetComponent<Image>();
        if (img != null)
            img.color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, alpha * arrowColor.a);
    }

    GameObject GetArrow()
    {
        GameObject arrow;
        if (_arrowPool.Count > 0)
        {
            arrow = _arrowPool[0];
            _arrowPool.RemoveAt(0);
        }
        else
        {
            arrow = Instantiate(arrowPrefab, hudCanvas.transform);
        }

        arrow.SetActive(true);
        _activeArrows.Add(arrow);
        return arrow;
    }

    void ReturnAllArrows()
    {
        foreach (GameObject arrow in _activeArrows)
        {
            arrow.SetActive(false);
            _arrowPool.Add(arrow);
        }
        _activeArrows.Clear();
    }
}