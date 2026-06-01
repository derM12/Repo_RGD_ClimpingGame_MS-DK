using UnityEngine;
using UnityEngine.InputSystem;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab;
    public float minForwardForce = 0.5f;
    public float maxForwardForce = 3f;
    public float chargeTime = 2f; // seconds to reach max force
    public float throwUpForce = 1.5f;

    PlayerInventory inventory;
    float chargeTimer = 0f;
    bool isCharging = false;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        // Start charging
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (inventory.rocks > 0)
            {
                isCharging = true;
                chargeTimer = 0f;
                Debug.Log("Charging throw...");
            }
            else
            {
                Debug.Log("No rocks left!");
            }
        }

        // While holding
        if (isCharging && Keyboard.current.rKey.isPressed)
        {
            chargeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(chargeTimer / chargeTime);
            float previewForce = Mathf.Lerp(minForwardForce, maxForwardForce, t);
            Debug.Log("Charge: " + (t * 100f).ToString("F0") + "% | Force: " + previewForce.ToString("F1"));
        }

        // Release to throw
        if (isCharging && Keyboard.current.rKey.wasReleasedThisFrame)
        {
            float t = Mathf.Clamp01(chargeTimer / chargeTime);
            float throwForce = Mathf.Lerp(minForwardForce, maxForwardForce, t);
            ThrowRock(throwForce);
            isCharging = false;
            chargeTimer = 0f;
        }
    }

    void ThrowRock(float forwardForce)
    {
        if (rockPrefab == null) return;

        if (!inventory.UseRock()) return; // stops throw if no rocks

        Vector3 spawnPos = transform.position + transform.forward * 0.5f + Vector3.up * 1f;
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = transform.forward * forwardForce + Vector3.up * throwUpForce;

        Debug.Log("Rock thrown with force: " + forwardForce.ToString("F1"));
    }
}