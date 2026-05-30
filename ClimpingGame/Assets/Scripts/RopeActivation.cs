using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class RopeActivation : MonoBehaviour
{
    public GameObject rope;
    public float interactRange = 2f;

    bool activated = false;
    FirstPersonController player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.GetComponent<FirstPersonController>();

        // Find rope as sibling (child of same parent)
        rope = transform.parent.Find("Rope").gameObject;

        if (rope != null)
            rope.SetActive(false);
        else
            Debug.LogError("HookActivator: No sibling named Rope found!");
    }
    void Update()
    {
        if (activated || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            rope.SetActive(true);
            activated = true;
            Debug.Log("Rope activated!");
        }
    }
}