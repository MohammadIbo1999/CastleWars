
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CameraFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    public float duration = 0.15f;
    public float width = 0.4f;

    private LineRenderer lineRenderer;
    private float remainingTime;
    private bool initialized = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }

    // Kamerablitz erzeugen
    public void Initialize(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        remainingTime = duration;
        initialized = true;

        lineRenderer.enabled = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        remainingTime -= Time.deltaTime;

        // Blitz langsam verkleinern
        float progress = Mathf.Clamp01(
            remainingTime / duration);

        lineRenderer.startWidth = width * progress;
        lineRenderer.endWidth = width * progress;

        // Effekt entfernen
        if (remainingTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
