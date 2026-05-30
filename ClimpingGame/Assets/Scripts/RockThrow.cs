using UnityEngine;
using UnityEngine.InputSystem;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab;
    public float throwForwardForce = 3f;
    public float throwUpForce = 4f;

    PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ThrowRock();
    }

    void ThrowRock()
    {
        if (rockPrefab == null) return;

        if (!inventory.UseRock()) return; // stops throw if no rocks

        Vector3 spawnPos = transform.position + transform.forward * 0.5f + Vector3.up * 1f;
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = transform.forward * throwForwardForce + Vector3.up * throwUpForce;
    }
}