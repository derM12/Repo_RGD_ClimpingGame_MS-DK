using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    public Volume globalVolume;

    Vignette vignette;

    public static VignetteController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (globalVolume.profile.TryGet(out vignette))
            Debug.Log("Vignette found!");
        else
            Debug.LogError("No Vignette override found on the Volume Profile!");
    }

    public void SetIntensity(float value)
    {
        if (vignette != null)
            vignette.intensity.value = Mathf.Clamp01(value);
    }
}