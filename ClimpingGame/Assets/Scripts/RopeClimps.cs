using UnityEngine;
using UnityEngine.InputSystem;

public class RopeClimb : MonoBehaviour
{
    public float interactRange = 2f;
    public Transform topPoint;

    PlayerMovement player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerMovement>();
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

        Vector3 checkFrom = topPoint != null ? topPoint.position : transform.position;
        float distance = Vector3.Distance(checkFrom, player.transform.position);

        Debug.Log("Distance to top of rope: " + distance + " | In range: " + (distance <= interactRange));

        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E pressed in range! IsClimbing: " + player.IsClimbing);
            if (player.IsClimbing)
                player.ExitClimb();
            else
                player.EnterClimb(transform);
        }
    }
}