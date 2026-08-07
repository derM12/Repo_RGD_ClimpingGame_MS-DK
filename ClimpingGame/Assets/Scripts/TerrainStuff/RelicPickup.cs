using UnityEngine;
using UnityEngine.InputSystem;

public class RelicPickup : MonoBehaviour
{
    public float interactRange = 2f;

    PlayerInventory inventory;
    bool pickedUp = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            inventory = playerObj.GetComponent<PlayerInventory>();
        else
            Debug.LogError("RelicPickup: No Player found!");
    }

    void Update()
    {
        if (pickedUp || inventory == null) return;

        float distance = Vector3.Distance(transform.position, inventory.transform.position);

        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            AudioManager.Instance?.PlayRelicPickup();
            inventory.AddRelic();
            pickedUp = true;
            gameObject.SetActive(false); // hide the relic
        }
    }
}