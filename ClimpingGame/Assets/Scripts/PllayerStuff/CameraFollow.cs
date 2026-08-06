using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float tiltWhenClimbing = 25f;
    public float smoothSpeed = 5f;

    float currentTilt;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow player
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);

        // Tilt down when climbing
        PlayerMovement pm = target.GetComponent<PlayerMovement>();
        float targetTilt = (pm != null && pm.IsClimbing) ? tiltWhenClimbing : 0f;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Euler(currentTilt, 0, 0);
    }
}