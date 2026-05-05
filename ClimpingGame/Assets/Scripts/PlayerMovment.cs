using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float climbSpeed = 2f;
    public float gravity = -9.81f;

    CharacterController cc;
    float yVelocity;
    Transform ropeTarget;
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

        // Snap X/Z to rope so player stays on it
        Vector3 snapped = new Vector3(ropeTarget.position.x, transform.position.y, ropeTarget.position.z);
        transform.position = Vector3.Lerp(transform.position, snapped, Time.deltaTime * 15f);

        cc.Move(Vector3.up * vertical * climbSpeed * Time.deltaTime);
        yVelocity = 0f;
    }

    public void EnterClimb(Transform rope)
    {
        IsClimbing = true;
        ropeTarget = rope;
        yVelocity = 0f;
    }

    public void ExitClimb()
    {
        IsClimbing = false;
        ropeTarget = null;
    }
}