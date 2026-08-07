using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Looping")]
    public AudioSource ropeSource;
    public AudioSource footstepSource;
    public AudioSource runSource;
    public AudioSource backgroundSource;

    [Header("One-Shot")]
    public AudioSource oneShotSource;
    public AudioSource fallSource;
    public AudioClip rockSmash;
    public AudioClip fallStart;
    public AudioClip relicPickup;
    public AudioClip tradingSound;

    [Header("Volumes")]
    [Range(0f, 1f)] public float ropeVolume = 1f;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 1f)] public float runVolume = 1f;
    [Range(0f, 1f)] public float backgroundVolume = 1f;
    [Range(0f, 1f)] public float oneShotVolume = 1f;
    [Range(0f, 1f)] public float fallVolume = 1f;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
        {
            if (ropeSource != null) ropeSource.volume = ropeVolume;
            if (footstepSource != null) footstepSource.volume = footstepVolume;
            if (runSource != null) runSource.volume = runVolume;
            if (backgroundSource != null) backgroundSource.volume = backgroundVolume;
            if (oneShotSource != null) oneShotSource.volume = oneShotVolume;

            if (backgroundSource != null)
            {
                backgroundSource.loop = true;
                backgroundSource.Play();
            }
        }
   

    // Looping controls
    public void SetRopeTension(bool playing) => SetLoop(ropeSource, playing);
    public void SetFootsteps(bool playing) => SetLoop(footstepSource, playing);
    public void SetRunning(bool playing) => SetLoop(runSource, playing);

    void SetLoop(AudioSource source, bool playing)
    {
        if (source == null) return;
        if (playing && !source.isPlaying) { source.loop = true; source.Play(); }
        else if (!playing && source.isPlaying) source.Stop();
    }

    // One shots
    public void PlayRockSmash() => PlayOneShot(rockSmash);

    public void PlayFallStart()
    {
        if (fallSource != null && fallStart != null)
        {
            fallSource.clip = fallStart;
            fallSource.loop = true; 
            fallSource.volume = fallVolume;
            fallSource.Play();
        }
    }

    public void StopFall()
    {
        if (fallSource != null && fallSource.isPlaying)
            fallSource.Stop();
    }

    public void PlayRelicPickup() => PlayOneShot(relicPickup);
    public void PlayTrading() => PlayOneShot(tradingSound);

    void PlayOneShot(AudioClip clip)
    {
        if (clip != null && oneShotSource != null)
            oneShotSource.PlayOneShot(clip);
    }
}