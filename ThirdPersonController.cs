using UnityEngine;
using Game.Weapons;

namespace Game.Locomotion
{
    /// <summary>
    /// High-level third person controller. It coordinates the input source
    /// (<see cref="ICharacterInput"/>) and the physics layer
    /// (<see cref="CharacterMotor"/>) but contains no input-reading nor
    /// raw CharacterController code itself.
    ///
    /// Responsibilities:
    ///  - Convert raw 2D input into a movement direction relative to the camera.
    ///  - Smoothly accelerate between walk and sprint speeds.
    ///  - Smoothly rotate the character to face its movement direction.
    ///  - Forward the resulting velocity and jump intent to the motor.
    ///  - Strictly limit speed to walking while a weapon is equipped/aiming.
    ///
    /// It depends on the <see cref="ICharacterInput"/> and (optional)
    /// <see cref="IWeaponState"/> abstractions rather than concrete classes,
    /// satisfying the Dependency Inversion Principle.
    /// </summary>
    [RequireComponent(typeof(CharacterMotor))]
    [DisallowMultipleComponent]
    public sealed class ThirdPersonController : MonoBehaviour, ILocomotionState
    {
        [Header("Movement Speeds (m/s)")]
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;

        [Tooltip("How quickly the character accelerates/decelerates toward the target speed. Higher = snappier.")]
        [SerializeField] private float _speedChangeRate = 12f;

        [Header("Rotation")]
        [Tooltip("Approximate time (seconds) to rotate toward the movement direction. Lower = snappier.")]
        [SerializeField] private float _rotationSmoothTime = 0.12f;

        [Header("Jump")]
        [Tooltip("Peak height of a jump, in meters.")]
        [SerializeField] private float _jumpHeight = 1.2f;

        [Header("References")]
        [Tooltip("Camera that movement is made relative to. Falls back to Camera.main if left empty.")]
        [SerializeField] private Transform _cameraTransform;

        private CharacterMotor _motor;
        private ICharacterInput _input;

        // Optional weapon state. When present and equipped, sprint is disabled so the
        // character is strictly limited to walking speed (DIP: depends on abstraction).
        private IWeaponState _weapon;

        private float _currentSpeed;
        private float _turnSmoothVelocity; // Working state for Mathf.SmoothDampAngle.

        // Squared threshold below which input is treated as "no movement".
        private const float InputDeadZoneSqr = 0.0001f;

        // --- ILocomotionState: read-only state published to the animation layer ---

        /// <inheritdoc />
        public event System.Action Jumped;

        /// <inheritdoc />
        /// <remarks>Current speed mapped into 0..1 using sprint speed as the maximum,
        /// so it can be fed straight into a 1D locomotion Blend Tree.</remarks>
        public float NormalizedSpeed => _sprintSpeed > 0f ? Mathf.Clamp01(_currentSpeed / _sprintSpeed) : 0f;

        /// <inheritdoc />
        public bool IsGrounded => _motor.IsGrounded;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();

            // Resolve the input source through the abstraction. Unity's GetComponent
            // supports interface lookups, keeping this class decoupled from the
            // concrete reader implementation.
            _input = GetComponent<ICharacterInput>();
            if (_input == null)
            {
                Debug.LogError(
                    $"{nameof(ThirdPersonController)} requires a component implementing {nameof(ICharacterInput)} " +
                    $"(e.g. {nameof(PlayerInputReader)}) on the same GameObject.", this);
                enabled = false;
                return;
            }

            // Optional: a weapon controller on the same GameObject lets us clamp speed
            // while aiming. Resolved through the abstraction; a null result is valid
            // (the character simply has no weapon and can always sprint).
            _weapon = GetComponent<IWeaponState>();

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        /// <summary>
        /// Optional manual dependency injection, useful for tests or for spawning
        /// the controller from code with a custom input source.
        /// </summary>
        public void Construct(ICharacterInput input, Transform cameraTransform = null)
        {
            _input = input;
            if (cameraTransform != null)
            {
                _cameraTransform = cameraTransform;
            }
        }

        private void Update()
        {
            Vector3 moveDirection = CalculateCameraRelativeDirection(_input.MoveInput);

            RotateTowards(moveDirection);

            Vector3 planarVelocity = CalculatePlanarVelocity(moveDirection);

            // Only jump (and notify the animation layer) when actually grounded, so
            // the Animator's "Jump" trigger never fires while already airborne.
            if (_input.JumpRequested && _motor.IsGrounded)
            {
                _motor.Jump(_jumpHeight);
                Jumped?.Invoke();
            }

            _motor.Move(planarVelocity);
        }

        /// <summary>
        /// Projects the 2D input onto the camera's flattened forward/right axes so
        /// that "up" on the stick always means "away from the camera", producing
        /// classic third-person, camera-relative movement.
        /// </summary>
        private Vector3 CalculateCameraRelativeDirection(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude < InputDeadZoneSqr)
            {
                return Vector3.zero;
            }

            // Without a camera, fall back to world-space directions.
            if (_cameraTransform == null)
            {
                return new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            }

            Vector3 forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

            return (forward * moveInput.y + right * moveInput.x).normalized;
        }

        /// <summary>
        /// Smoothly rotates the character around the Y axis to face the given
        /// horizontal direction. Does nothing when there is no movement so the
        /// character keeps its last facing while idle.
        /// </summary>
        private void RotateTowards(Vector3 direction)
        {
            if (direction.sqrMagnitude < InputDeadZoneSqr)
            {
                return;
            }

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }

        /// <summary>
        /// Picks the target speed (walk or sprint), then eases the current speed
        /// toward it for smooth acceleration/deceleration. Returns the resulting
        /// horizontal velocity vector.
        /// </summary>
        private Vector3 CalculatePlanarVelocity(Vector3 direction)
        {
            bool hasInput = direction.sqrMagnitude >= InputDeadZoneSqr;

            // Sprinting is only allowed when NOT aiming/equipped. With a weapon out,
            // the character is strictly capped at walking speed regardless of input.
            bool weaponEquipped = _weapon != null && _weapon.IsEquipped;
            bool sprinting = _input.SprintHeld && !weaponEquipped;

            float maxSpeed = sprinting ? _sprintSpeed : _walkSpeed;
            float targetSpeed = hasInput ? maxSpeed : 0f;

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _speedChangeRate * Time.deltaTime);

            return direction * _currentSpeed;
        }
    }
}
