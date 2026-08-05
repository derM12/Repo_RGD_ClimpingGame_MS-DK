using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Climbing")]
        public float ClimbSpeed = 2f;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // climbing
        public bool IsClimbing { get; private set; }
        private Transform _ropeTarget;
        private float _ropeMinY;
        private float _ropeMaxY;
        private Vector3 _ropeExitDirection;
        private bool _enteredFromBottom;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (IsClimbing)
                Climb();
            else
            {
                JumpAndGravity();
                GroundedCheck();
                Move();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // only rotate player left/right when not climbing
                if (!IsClimbing)
                    transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            if (!_controller.enabled) return;

            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void Climb()
        {
            // W/S moves up and down the rope
            float vertical = _input.move.y;

            // Snap X/Z to rope
            Vector3 snapped = new Vector3(_ropeTarget.position.x, transform.position.y, _ropeTarget.position.z);
            transform.position = Vector3.Lerp(transform.position, snapped, Time.deltaTime * 15f);

            _controller.Move(Vector3.up * vertical * ClimbSpeed * Time.deltaTime);

            // Clamp to rope bounds
            float clampedY = Mathf.Clamp(transform.position.y, _ropeMinY, _ropeMaxY);
            if (transform.position.y != clampedY)
            {
                Vector3 pos = transform.position;
                pos.y = clampedY;
                transform.position = pos;
            }

            _verticalVelocity = 0f;
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += Gravity * Time.deltaTime;
        }

        public void EnterClimb(Transform rope, Transform topPoint, Transform bottomPoint, bool fromBottom = false)
        {
            IsClimbing = true;
            _ropeTarget = rope;
            _verticalVelocity = 0f;

            _ropeMaxY = topPoint != null ? topPoint.position.y : rope.position.y;
            _ropeMinY = bottomPoint != null ? bottomPoint.position.y : rope.position.y - 10f;

            // Snap to rope's clean forward or backward axis
            Vector3 directionFromRope = transform.position - rope.position;
            directionFromRope.y = 0f;
            directionFromRope.Normalize();

            float dot = Vector3.Dot(rope.right, directionFromRope);
            Vector3 snappedDir = dot >= 0 ? rope.right : -rope.right;

            _ropeExitDirection = snappedDir;
            _enteredFromBottom = fromBottom;

            if (fromBottom)
            {
                // Stay in place, just smooth rotate to face the rope
                StartCoroutine(SmoothRotate(Quaternion.LookRotation(-snappedDir), 0.5f));
                Debug.Log("Grabbed from bottom");
            }
            else
            {
                // Flip to opposite side at top
                float grabDistance = 1f;
                Vector3 opposite = rope.position + (-snappedDir * grabDistance);
                opposite.y = _ropeMaxY;

                _controller.enabled = false;
                transform.position = opposite;
                Physics.SyncTransforms();
                _controller.enabled = true;

                StartCoroutine(SmoothRotate(Quaternion.LookRotation(snappedDir), 0.5f));
                Debug.Log("Grabbed from top - snapped to clean axis");
            }
        }

        public void ExitClimb()
        {
            bool atTop = Mathf.Abs(transform.position.y - _ropeMaxY) < 0.1f;
            bool shouldFlip = atTop || _enteredFromBottom;

            IsClimbing = false;
            _verticalVelocity = 0f;

            if (shouldFlip)
            {
                float grabDistance = 1f;

                // Bottom entry: flip to opposite side. Top entry: flip back to original side.
                Vector3 flipDir = _enteredFromBottom ? -_ropeExitDirection : _ropeExitDirection;

                Vector3 exitPos = _ropeTarget.position + (flipDir * grabDistance);
                exitPos.y = transform.position.y;

                StartCoroutine(TeleportExit(exitPos, Quaternion.LookRotation(flipDir), true));
                Debug.Log("Exited - flipping to: " + flipDir);
            }
            else
            {
                StartCoroutine(SmoothRotate(Quaternion.LookRotation(_ropeExitDirection), 0.5f));
                Debug.Log("Exited mid-rope - staying in place");
            }

            _ropeTarget = null;
            _enteredFromBottom = false;
        }

        private IEnumerator TeleportExit(Vector3 position, Quaternion targetRotation, bool smoothRot)
        {
            _controller.enabled = false;
            transform.position = position;
            Physics.SyncTransforms();
            yield return null;
            _controller.enabled = true;

            if (smoothRot)
                StartCoroutine(SmoothRotate(targetRotation, 0.5f));
        }

        private IEnumerator SmoothRotate(Quaternion targetRotation, float duration)
        {
            Quaternion startRotation = transform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
                yield return null;
            }

            transform.rotation = targetRotation;
        }

        public bool IsOnRope(Transform rope)
        {
            return _ropeTarget == rope;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}