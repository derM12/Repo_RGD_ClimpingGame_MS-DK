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
    float recoveryTimer = 0f;
    bool isRecovering = false;

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

        TrackFall();
        HandleRecovery();
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

            // Live trauma while falling
            float currentFallDistance = highestY - transform.position.y;
            if (currentFallDistance > safeFallHeight)
            {
                float t = Mathf.InverseLerp(safeFallHeight, deadlyFallHeight, currentFallDistance);
                trauma = Mathf.Clamp01(t);
                isRecovering = false;
                recoveryTimer = 0f;

                // Live percentage logs
                if (trauma >= 1f)
                    Debug.Log("Falling | Trauma: 100% - FATAL");
                else if (trauma > 0.66f)
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - CRITICAL");
                else if (trauma > 0.33f)
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - HEAVY");
                else
                    Debug.Log("Falling | Trauma: " + (trauma * 100f).ToString("F0") + "% - LIGHT");
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
                trauma = 0f;
                Debug.Log("Safe landing.");
            }

            highestY = transform.position.y;
            wasFalling = false;
        }
    }

    void ApplyTrauma(float amount, float fallDistance)
    {
        trauma = Mathf.Clamp01(amount); // use directly, not additive, since live fall already set it
        recoveryTimer = 0f;
        isRecovering = false;

        Debug.Log("Landed! Fell " + fallDistance.ToString("F1") + " units | Trauma: " + (trauma * 100f).ToString("F0") + "%");

        if (trauma >= 1f)
        {
            Debug.Log("DEAD");
            // GameManager.Instance.RestartGame();
        }
        else if (trauma > 0.66f)
            Debug.Log("Vignette: CRITICAL");
        else if (trauma > 0.33f)
            Debug.Log("Vignette: HEAVY");
        else
            Debug.Log("Vignette: LIGHT");
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

            // Log recovery milestones
            if (trauma <= 0f)
                Debug.Log("Vignette: fully recovered");
        }
    }
}