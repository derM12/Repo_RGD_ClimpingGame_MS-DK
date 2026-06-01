using UnityEngine;
using UnityEngine.InputSystem;

public class Shopkeeper : MonoBehaviour
{
    public float interactRange = 2f;

    PlayerInventory inventory;
    Animator animator;


    void Start()
    {
            animator = GetComponent<Animator>();
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
        {
            if (inventory.TradeRelicForRock())
            {
                if (animator != null)
                    animator.SetTrigger("Sell");
            }
        }
    }
}