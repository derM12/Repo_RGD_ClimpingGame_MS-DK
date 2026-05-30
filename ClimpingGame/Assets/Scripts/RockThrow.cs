using UnityEngine;
using UnityEngine.InputSystem;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab;
    public float throwForwardForce = 3f;
    public float throwUpForce = 4f;

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ThrowRock();
    }

    void ThrowRock()
    {
        if (rockPrefab == null) return;

        // Spawn just in front and slightly above the player
        Vector3 spawnPos = transform.position + transform.forward * 0.5f + Vector3.up * 1f;
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDir = transform.forward * throwForwardForce + Vector3.up * throwUpForce;
            rb.linearVelocity = throwDir;
        }
    }
}