using UnityEngine;
using UnityEngine.InputSystem;

public class Shopkeeper : MonoBehaviour
{
    public float interactRange = 2f;

    PlayerInventory inventory;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            inventory = playerObj.GetComponent<PlayerInventory>();
        else
            Debug.LogError("Shopkeeper: No Player found!");
    }

    void Update()
    {
        if (inventory == null) return;

        float distance = Vector3.Distance(transform.position, inventory.transform.position);

        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
            inventory.TradeRelicForRock();
    }
}