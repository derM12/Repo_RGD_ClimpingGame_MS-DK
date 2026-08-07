using UnityEngine;

public class RandomAmbience : MonoBehaviour
{
    [Header("Crow")]
    public AudioClip crowCaw;
    public float crowMinInterval = 15f;
    public float crowMaxInterval = 45f;

    [Header("Breathing")]
    public AudioClip creepyBreathing;
    public float breathingMinInterval = 30f;
    public float breathingMaxInterval = 90f;

    [Header("Wind")]
    public AudioClip windGust;
    public float windMinInterval = 10f;
    public float windMaxInterval = 30f;

    [Header("Volumes")]
    [Range(0f, 1f)] public float crowVolume = 0.5f;
    [Range(0f, 1f)] public float breathingVolume = 0.3f;
    [Range(0f, 1f)] public float windVolume = 0.6f;

    AudioSource source;
    float crowTimer;
    float breathingTimer;
    float windTimer;

    void Start()
    {
        source = GetComponent<AudioSource>();
        RandomizeTimers();
    }

    void RandomizeTimers()
    {
        crowTimer = Random.Range(crowMinInterval, crowMaxInterval);
        breathingTimer = Random.Range(breathingMinInterval, breathingMaxInterval);
        windTimer = Random.Range(windMinInterval, windMaxInterval);
    }

    void Update()
    {
        crowTimer -= Time.deltaTime;
        breathingTimer -= Time.deltaTime;
        windTimer -= Time.deltaTime;

        if (crowTimer <= 0f)
        {
            source.PlayOneShot(crowCaw, crowVolume);
            crowTimer = Random.Range(crowMinInterval, crowMaxInterval);
        }

        if (breathingTimer <= 0f)
        {
            source.PlayOneShot(creepyBreathing, breathingVolume);
            breathingTimer = Random.Range(breathingMinInterval, breathingMaxInterval);
        }

        if (windTimer <= 0f)
        {
            source.PlayOneShot(windGust, windVolume);
            windTimer = Random.Range(windMinInterval, windMaxInterval);
        }
    }
}