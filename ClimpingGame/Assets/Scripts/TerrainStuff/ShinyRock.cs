using UnityEngine;

public class ShinyRock : MonoBehaviour
{
    [Header("Color Settings")]
    public Color startColor = Color.green;
    public Color midColor = Color.yellow;
    public Color endColor = Color.red;
    public float colorChangeDistance = 10f; // adjust this in inspector

    [Header("Lifetime")]
    public float fadeStartTime = 30f;
    public float fadeEndTime = 60f;

    float timeAlive = 0f;

    float startY;
    Renderer rockRenderer;
    MaterialPropertyBlock propBlock;

    void Start()
    {
        startY = transform.position.y;
        rockRenderer = GetComponentInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        float fallen = startY - transform.position.y;
        float t = Mathf.Clamp01(fallen / (colorChangeDistance * 2f));

        // First half = start to mid, second half = mid to end
        Color currentColor;
        if (t < 0.5f)
            currentColor = Color.Lerp(startColor, midColor, t * 2f);
        else
            currentColor = Color.Lerp(midColor, endColor, (t - 0.5f) * 2f);

        rockRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", currentColor);
        propBlock.SetColor("_EmissionColor", currentColor * 0.5f);
        rockRenderer.SetPropertyBlock(propBlock);

        // Lifetime
        timeAlive += Time.deltaTime;

        if (timeAlive >= fadeEndTime)
        {
            Destroy(gameObject);
            return;
        }

        if (timeAlive >= fadeStartTime)
        {
            float fadeT = Mathf.InverseLerp(fadeStartTime, fadeEndTime, timeAlive);
            float alpha = Mathf.Lerp(1f, 0f, fadeT);

            Color fadedColor = currentColor;
            fadedColor.a = alpha;

            rockRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", fadedColor);
            propBlock.SetColor("_EmissionColor", fadedColor * 0.5f);
            rockRenderer.SetPropertyBlock(propBlock);

        }
    }
}