using UnityEngine;

public class FogController: MonoBehaviour
{
    [Header("Depth Settings")]
    [Tooltip("Y position where fog starts getting thicker")]
    public float surfaceY = 0f;
    [Tooltip("Y position where fog reaches maximum thickness")]
    public float maxDepthY = -50f;

    [Header("Fog Density")]
    [Tooltip("Fog density at the surface")]
    public float minDensity = 0.01f;
    [Tooltip("Fog density at max depth")]
    public float maxDensity = 0.1f;

    void Update()
    {
        float t = Mathf.InverseLerp(surfaceY, maxDepthY, transform.position.y);
        RenderSettings.fogDensity = Mathf.Lerp(minDensity, maxDensity, t);
    }
}