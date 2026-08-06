using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLedgePeek : MonoBehaviour
{
    [Header("Peek Settings")]
    public float loweredOffset = -0.5f;     // Y drop
    public float downwardTiltAdd = 20f;     // degrees added to current pitch
    public float forwardOffset = 0.5f;      // how far forward camera extends
    public float peekSpeed = 5f;

    FirstPersonController fpc;
    float currentTilt = 0f;
    float currentForward = 0f;

    void Start()
    {
        fpc = GetComponentInParent<FirstPersonController>();
    }

    void Update()
    {
        bool peeking = Keyboard.current.leftCtrlKey.isPressed;

        float targetTilt = peeking ? downwardTiltAdd : 0f;
        float targetForward = peeking ? forwardOffset : 0f;

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * peekSpeed);
        currentForward = Mathf.Lerp(currentForward, targetForward, Time.deltaTime * peekSpeed);

        fpc.peekTiltOffset = currentTilt;
        fpc.peekForwardOffset = currentForward;
    }
}