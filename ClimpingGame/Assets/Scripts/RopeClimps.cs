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
                // Check top point
                float distanceTop = topPoint != null ? Vector3.Distance(topPoint.position, player.transform.position) : float.MaxValue;
                // Check bottom point
                float distanceBottom = bottomPoint != null ? Vector3.Distance(bottomPoint.position, player.transform.position) : float.MaxValue;

                if (distanceTop <= interactRange)
                {
                    Debug.Log("Grabbing from top");
                    player.EnterClimb(transform, topPoint, bottomPoint, fromBottom: false);
                }
                else if (distanceBottom <= interactRange)
                {
                    Debug.Log("Grabbing from bottom");
                    player.EnterClimb(transform, topPoint, bottomPoint, fromBottom: true);
                }
                else
                {
                    Debug.Log("Distance top: " + distanceTop + " | Distance bottom: " + distanceBottom);
                }
            }
        }
    }
}