using UnityEngine;
using UnityEngine.InputSystem;

public class PickaxeBuy : MonoBehaviour
{
    public float interactRange = 2f;

    private PlayerInventory inventory;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            inventory = player.GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (inventory == null)
            return;

        float distance = Vector3.Distance(transform.position, inventory.transform.position);

        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            inventory.BuyPickaxe();
        }
    }
}
