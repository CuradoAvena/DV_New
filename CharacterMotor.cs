using UnityEngine;

namespace Game.Locomotion
{
    /// <summary>
    /// Owns the <see cref="CharacterController"/> and is solely responsible for
    /// applying motion, gravity and jumping (Single Responsibility Principle).
    ///
    /// It knows nothing about input or cameras — it just receives a desired
    /// planar velocity and an optional jump request and resolves the physics.
    /// This keeps movement decisions (the "what") separate from physics
    /// execution (the "how").
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    public sealed class CharacterMotor : MonoBehaviour
    {
        [Header("Gravity")]
        [Tooltip("Downward acceleration applied while airborne (m/s^2). Negative.")]
        [SerializeField] private float _gravity = -19.62f;

        [Tooltip("Small constant downward velocity while grounded to keep the controller snapped to the ground.")]
        [SerializeField] private float _groundedStickVelocity = -2f;

        private CharacterController _controller;
        private float _verticalVelocity;
        private bool _jumpQueued;
        private float _queuedJumpHeight;

        /// <summary>True when the controller is resting on the ground this frame.</summary>
        public bool IsGrounded => _controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Queues a jump to be executed on the next <see cref="Move"/> call.
        /// The jump only takes effect if the character is grounded at that time.
        /// </summary>
        /// <param name="jumpHeight">Desired peak height of the jump, in meters.</param>
        public void Jump(float jumpHeight)
        {
            _jumpQueued = true;
            _queuedJumpHeight = jumpHeight;
        }

        /// <summary>
        /// Advances the character by the given planar (XZ) velocity for this frame,
        /// resolving gravity and any queued jump internally. Should be called once
        /// per frame from the controlling component.
        /// </summary>
        /// <param name="planarVelocity">Desired horizontal velocity in world space (m/s). Y is ignored.</param>
        public void Move(Vector3 planarVelocity)
        {
            UpdateVerticalVelocity();

            Vector3 velocity = new Vector3(planarVelocity.x, _verticalVelocity, planarVelocity.z);
            _controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Integrates gravity and applies a queued jump. While grounded the vertical
        /// velocity is clamped to a small stick value; a jump converts the desired
        /// height into an initial upward velocity via v = sqrt(2 * g * h).
        /// </summary>
        private void UpdateVerticalVelocity()
        {
            if (_controller.isGrounded)
            {
                _verticalVelocity = _groundedStickVelocity;

                if (_jumpQueued && _queuedJumpHeight > 0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_queuedJumpHeight * -2f * _gravity);
                }
            }
            else
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }

            // A jump request only ever applies to the frame it was made.
            _jumpQueued = false;
        }
    }
}
