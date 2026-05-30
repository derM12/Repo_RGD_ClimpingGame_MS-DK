using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class RopeClimb : MonoBehaviour
{
    public float interactRange = 2f;
    public Transform topPoint;
    public Transform bottomPoint;

    FirstPersonController player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<FirstPersonController>();
            Debug.Log("RopeClimb: Player found - " + playerObj.name);
        }
        else
        {
            Debug.LogError("RopeClimb: No GameObject with tag 'Player' found!");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (player.IsClimbing)
            {
                if (player.IsOnRope(transform))
                {
                    player.ExitClimb();
                    Debug.Log("Exited climb");
                }
            }
            else
            {
                Vector3 checkFrom = topPoint != null ? topPoint.position : transform.position;
                float distance = Vector3.Distance(checkFrom, player.transform.position);
                Debug.Log("Distance to top of rope: " + distance + " | In range: " + (distance <= interactRange));

                if (distance <= interactRange)
                {
                    player.EnterClimb(transform, topPoint, bottomPoint);
                    Debug.Log("Entered climb");
                }
            }
        }
    }
}