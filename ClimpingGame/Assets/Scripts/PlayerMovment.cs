using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float climbSpeed = 2f;
    public float gravity = -9.81f;
    Vector3 ropeExitDirection;

    CharacterController cc;
    float yVelocity;
    Transform ropeTarget;
    float ropeMinY;
    float ropeMaxY;
    public bool IsClimbing { get; private set; }

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsClimbing)
            Climb();
        else
            Walk();
    }

    void Walk()
    {
        if (!cc.enabled) return; // wait until CC is back on

        var kb = Keyboard.current;
        float x = 0f, z = 0f;
        if (kb.wKey.isPressed) z = 1f;
        if (kb.sKey.isPressed) z = -1f;
        if (kb.aKey.isPressed) x = -1f;
        if (kb.dKey.isPressed) x = 1f;

        Transform cam = Camera.main.transform;
        Vector3 forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 right = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 move = (forward * z + right * x) * walkSpeed;

        if (cc.isGrounded && yVelocity < 0f) yVelocity = -2f;
        yVelocity += gravity * Time.deltaTime;

        cc.Move((move + Vector3.up * yVelocity) * Time.deltaTime);
    }

    void Climb()
    {
        var kb = Keyboard.current;
        float vertical = 0f;
        if (kb.wKey.isPressed) vertical = 1f;
        if (kb.sKey.isPressed) vertical = -1f;

        // Snap X/Z to rope
        Vector3 snapped = new Vector3(ropeTarget.position.x, transform.position.y, ropeTarget.position.z);
        transform.position = Vector3.Lerp(transform.position, snapped, Time.deltaTime * 15f);

        cc.Move(Vector3.up * vertical * climbSpeed * Time.deltaTime);

        // Just clamp, don't exit
        float clampedY = Mathf.Clamp(transform.position.y, ropeMinY, ropeMaxY);
        if (transform.position.y != clampedY)
        {
            Vector3 pos = transform.position;
            pos.y = clampedY;
            transform.position = pos;
        }

        yVelocity = 0f;
    }

    public void EnterClimb(Transform rope, Transform topPoint, Transform bottomPoint)
    {
        IsClimbing = true;
        ropeTarget = rope;
        yVelocity = 0f;

        ropeMaxY = topPoint != null ? topPoint.position.y : rope.position.y;
        ropeMinY = bottomPoint != null ? bottomPoint.position.y : rope.position.y - 10f;

        Vector3 directionFromRope = transform.position - rope.position;
        directionFromRope.y = 0f;
        directionFromRope.Normalize();

        // Store the OPPOSITE direction for exit later
        ropeExitDirection = directionFromRope;

        float grabDistance = 1f;
        Vector3 opposite = rope.position + (-directionFromRope * grabDistance);
        opposite.y = ropeMaxY;

        cc.enabled = false;
        transform.position = opposite;
        transform.rotation = Quaternion.LookRotation(-directionFromRope);
        cc.enabled = true;

        Debug.Log("Entered climb. MinY: " + ropeMinY + " MaxY: " + ropeMaxY);
    }

    public bool IsOnRope(Transform rope)
    {
        return ropeTarget == rope;
    }

    public void ExitClimb()
    {
        float grabDistance = 1f;
        Vector3 exitPos = ropeTarget.position + (ropeExitDirection * grabDistance);
        exitPos.y = transform.position.y;

        IsClimbing = false;
        ropeTarget = null;
        yVelocity = 0f;

        cc.enabled = false;
        transform.position = exitPos;
        transform.rotation = Quaternion.LookRotation(ropeExitDirection);
        Physics.SyncTransforms();
        cc.enabled = true;

        Debug.Log("Exited climb");
    }
}