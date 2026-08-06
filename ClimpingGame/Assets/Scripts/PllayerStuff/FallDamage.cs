using UnityEngine;
using StarterAssets;

public class FallDamage : MonoBehaviour
{
    [Header("Fall Settings")]
    public float safeFallHeight = 5f;      // below this, no damage
    public float deadlyFallHeight = 20f;   // instant death

    [Header("Recovery")]
    public float recoveryDelay = 5f;       // seconds before recovery starts
    public float recoverySpeed = 0.05f;    // trauma lost per second

    float trauma = 0f;                     // 0 = fine, 1 = dead
    float previousTrauma = 0f;             // trauma carried over from previous falls
    float recoveryTimer = 0f;
    bool isRecovering = false;
    bool justRespawned = false;            // blocks TrackFall right after respawn

    float highestY;
    bool wasFalling = false;

    FirstPersonController player;
    CharacterController cc;

    void Start()
    {
        player = GetComponent<FirstPersonController>();
        cc = GetComponent<CharacterController>();
        highestY = transform.position.y;
    }

    void Update()
    {
        if (player.IsClimbing)
        {
            // Reset tracking while on rope
            highestY = transform.position.y;
            wasFalling = false;
            return;
        }

        if (!justRespawned)
            TrackFall();
        else if (!cc.isGrounded && cc.velocity.y < -0.1f)
        {
            // Player started falling after respawn - resume tracking
            justRespawned = false;
            highestY = transform.position.y;
            Debug.Log("Respawn immunity cancelled - falling detected");
        }

        HandleRecovery();

        // Clear flag once fully recovered after respawn
        if (justRespawned && trauma <= 0f)
            justRespawned = false;
    }

    void TrackFall()
    {
        if (cc.isGrounded || cc.velocity.y > 0f)
        {
            if (!wasFalling)
                highestY = transform.position.y;
        }

        bool falling = !cc.isGrounded && cc.velocity.y < -0.1f;

        if (falling)
        {
            wasFalling = true;

            float currentFallDistance = highestY - transform.position.y;
            if (currentFallDistance > safeFallHeight)
            {
                float t = Mathf.InverseLerp(safeFallHeight, deadlyFallHeight, currentFallDistance);
                trauma = Mathf.Clamp01(previousTrauma + t); // add to previous trauma
                isRecovering = false;
                recoveryTimer = 0f;

                if (trauma >= 1f)
                {
                    Debug.Log("DEAD mid-air - respawning");
                    wasFalling = false;
                    highestY = transform.position.y;
                    Respawn();
                    return;
                }

                if (trauma > 0.66f)
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - CRITICAL");
                else if (trauma > 0.33f)
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - HEAVY");
                else
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - LIGHT");

                VignetteController.Instance?.SetIntensity(trauma);
            }
        }

        // Landed
        if (wasFalling && cc.isGrounded)
        {
            float fallDistance = highestY - transform.position.y;

            if (fallDistance > safeFallHeight)
            {
                float t = Mathf.InverseLerp(safeFallHeight, deadlyFallHeight, fallDistance);
                ApplyTrauma(t, fallDistance); // final landed result
            }
            else
            {
                Debug.Log("Safe landing.");
            }

            previousTrauma = trauma; // save trauma after landing
            highestY = transform.position.y;
            wasFalling = false;
        }
    }

    void ApplyTrauma(float amount, float fallDistance)
    {
        trauma = Mathf.Clamp01(previousTrauma + amount); // add to previous trauma
        recoveryTimer = 0f;
        isRecovering = false;

        Debug.Log("Landed! Fell " + fallDistance.ToString("F1") + " units | Trauma: " + (trauma * 100f).ToString("F0") + "%");

        if (trauma >= 1f)
        {
            Debug.Log("DEAD");
            Respawn();
        }
        else if (trauma > 0.66f)
            Debug.Log("Vignette: CRITICAL");
        else if (trauma > 0.33f)
            Debug.Log("Vignette: HEAVY");
        else
            Debug.Log("Vignette: LIGHT");

        VignetteController.Instance?.SetIntensity(trauma);
    }

    void HandleRecovery()
    {
        if (trauma <= 0f) return;

        if (!isRecovering)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryDelay)
            {
                isRecovering = true;
                Debug.Log("Vignette: starting to recover...");
            }
        }
        else
        {
            trauma = Mathf.Max(0f, trauma - recoverySpeed * Time.deltaTime);
            previousTrauma = trauma;
            VignetteController.Instance?.SetIntensity(trauma); // drives the fade out

            if (trauma <= 0f)
            {
                previousTrauma = 0f;
                VignetteController.Instance?.SetIntensity(0f);
                Debug.Log("Vignette: fully recovered");
            }
        }
    }

    void Respawn()
    {
        if (SpawnPoint.Instance == null)
        {
            Debug.LogError("No SpawnPoint found in scene!");
            return;
        }

        previousTrauma = 0f;
        wasFalling = false;
        justRespawned = true;  // block TrackFall until recovered
        isRecovering = true;   // skip delay, recover immediately
        recoveryTimer = 0f;
        highestY = SpawnPoint.Instance.transform.position.y;

        cc.enabled = false;
        transform.position = SpawnPoint.Instance.transform.position;
        transform.rotation = SpawnPoint.Instance.transform.rotation;
        Physics.SyncTransforms();
        cc.enabled = true;

        Debug.Log("Respawned - recovering");
    }
}