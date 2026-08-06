using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private Vector3 velocity;

    private bool moving = false;
    private bool isUp = false;

    public Vector3 DeltaMovement { get; private set; }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                0.2f,
                moveSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                moving = false;
            }
        }

        DeltaMovement = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (moving) return;

        if (isUp)
        {
            targetPosition = startPosition;
            isUp = false;
        }
        else
        {
            targetPosition = startPosition + Vector3.up * moveDistance;
            isUp = true;
        }

        moving = true;
    }
}