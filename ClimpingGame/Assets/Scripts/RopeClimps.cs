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
                //check top Point
                float distanceTop = topPoint != null ? Vector3.Distance(topPoint.position, player.transform.position) : float.MaxValue;
                //check bottom Point
                float distanceBottom = bottomPoint != null ? Vector3.Distance(bottomPoint.position, player.transform.position) : float.MaxValue;


                if (distanceTop <= interactRange || distanceBottom <= interactRange)
                {
                    // Check approach angle
                    Vector3 dirToPlayer = (player.transform.position - transform.position);
                    dirToPlayer.y = 0f;
                    dirToPlayer.Normalize();

                    float angle = Vector3.Angle(transform.right, dirToPlayer);

                    // Only allow from front (0-45°) or back (135-180°)
                    if (angle <= 45f || angle >= 135f)
                    {
                        bool fromBottom = distanceBottom < distanceTop;
                        player.EnterClimb(transform, topPoint, bottomPoint, fromBottom);
                        Debug.Log("Entered climb at angle: " + angle);
                    }
                    else
                    {
                        Debug.Log("Bad angle: " + angle + " - approach from front or back of rope");
                    }
                }
            }
        }
    }
}