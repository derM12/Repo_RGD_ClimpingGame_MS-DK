using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool moving = false;
    private bool isUp = false;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
    }

    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                moving = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (moving)
            return;

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