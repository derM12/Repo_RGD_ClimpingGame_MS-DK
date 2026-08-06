using UnityEngine;
using UnityEngine.UIElements;

public class PickaxeSwing : MonoBehaviour
{
    public float swingAngle = 50f;
    public float swingDuration = 0.5f;
    private float swingTimer = 0f;
    private Quaternion startRotation;
    private bool isSwinging = false;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            StartSwing();
        }

        if (isSwinging)
        {
            UpdateSwing();
        }
    }

    private void StartSwing()
    {
        isSwinging = true;
        swingTimer = 0f;
        startRotation = transform.localRotation;
    }

    private void UpdateSwing()
    {
        swingTimer += Time.deltaTime;
        float progress = swingTimer / swingDuration;

        if (progress >= 1f)
        {
            transform.localRotation = startRotation;
            isSwinging = false;
            return;
        }

        // Swing forward and back on Z axis in local space
        float currentAngle = Mathf.Sin(progress * Mathf.PI) * swingAngle;
        transform.localRotation = startRotation * Quaternion.Euler(0, 0, currentAngle);
    }
}
