using UnityEngine;

public class TriggerMovePlane : MonoBehaviour
{
    public GameObject plane;
    public float moveY = 2f;
    public float speed = 15f;

    bool triggered = false;
    Vector3 targetPos;

    void Start()
    {
        targetPos = plane.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPos = plane.transform.position + Vector3.up * moveY;
            triggered = true;
        }
    }

    void Update()
    {
        if (!triggered) return;
        plane.transform.position = Vector3.MoveTowards(plane.transform.position, targetPos, speed * Time.deltaTime);
    }
}