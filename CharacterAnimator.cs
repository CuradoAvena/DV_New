using UnityEngine;

namespace Game.Locomotion
{
    /// <summary>
    /// Bridges the movement layer to the Mecanim Animator. It is the ONLY class
    /// that talks to the <see cref="Animator"/> (Single Responsibility Principle),
    /// and it does so purely by reading the <see cref="ILocomotionState"/>
    /// abstraction — it never references the CharacterController or input.
    ///
    /// Designed for Mixamo "In Place" humanoid clips with NO root motion: the
    /// movement is driven 100% by code, and this script only reflects that motion
    /// visually by feeding Animator parameters:
    ///  - "Speed"   (Float, 0..1)  -> drives a 1D locomotion Blend Tree.
    ///  - "Jump"    (Trigger)      -> fired once when a jump starts.
    ///  - "Grounded"(Bool, optional) -> set if the parameter exists in the controller.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public sealed class CharacterAnimator : MonoBehaviour
    {
        [Header("Animator Parameter Names")]
        [Tooltip("Float parameter driving the 1D locomotion Blend Tree (0 = idle, 1 = sprint).")]
        [SerializeField] private string _speedParameter = "Speed";

        [Tooltip("Trigger parameter fired the moment a jump starts.")]
        [SerializeField] private string _jumpParameter = "Jump";

        [Tooltip("Optional Bool parameter mirroring the grounded state. Ignored if the Animator has no such parameter.")]
        [SerializeField] private string _groundedParameter = "Grounded";

        [Header("Smoothing")]
        [Tooltip("Damping time (seconds) applied to the Speed parameter so blend-tree transitions stay smooth.")]
        [SerializeField] private float _speedDampTime = 0.1f;

        private Animator _animator;
        private ILocomotionState _locomotion;

        // Parameter name hashes are cached once; they are far cheaper than passing
        // strings to the Animator every frame.
        private int _speedHash;
        private int _jumpHash;
        private int _groundedHash;
        private bool _hasGroundedParameter;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            // The Animator usually sits on the same object as the movement scripts,
            // but on imported Mixamo rigs it can be on a child while movement lives
            // on the root. GetComponentInParent covers both layouts (it includes self).
            _locomotion = GetComponentInParent<ILocomotionState>();
            if (_locomotion == null)
            {
                Debug.LogError(
                    $"{nameof(CharacterAnimator)} requires a component implementing {nameof(ILocomotionState)} " +
                    $"(e.g. {nameof(ThirdPersonController)}) on this object or a parent.", this);
                enabled = false;
                return;
            }

            _speedHash = Animator.StringToHash(_speedParameter);
            _jumpHash = Animator.StringToHash(_jumpParameter);
            _groundedHash = Animator.StringToHash(_groundedParameter);
            _hasGroundedParameter = HasParameter(_groundedHash);
        }

        /// <summary>Subscribe to the one-shot jump event only while active.</summary>
        private void OnEnable()
        {
            if (_locomotion != null)
            {
                _locomotion.Jumped += HandleJumped;
            }
        }

        /// <summary>Always unsubscribe to avoid dangling handlers / leaks.</summary>
        private void OnDisable()
        {
            if (_locomotion != null)
            {
                _locomotion.Jumped -= HandleJumped;
            }
        }

        /// <summary>
        /// Pushes the continuous locomotion values into the Animator every frame.
        /// SetFloat with a damp time gives the blend tree a smooth ramp even if the
        /// underlying speed changes abruptly.
        /// </summary>
        private void Update()
        {
            _animator.SetFloat(_speedHash, _locomotion.NormalizedSpeed, _speedDampTime, Time.deltaTime);

            if (_hasGroundedParameter)
            {
                _animator.SetBool(_groundedHash, _locomotion.IsGrounded);
            }
        }

        /// <summary>Fires the Animator's "Jump" trigger in response to the movement event.</summary>
        private void HandleJumped()
        {
            _animator.SetTrigger(_jumpHash);
        }

        /// <summary>
        /// Returns true if the Animator controller actually declares a parameter with
        /// the given hash. Used to make the optional "Grounded" bool fully safe to omit.
        /// </summary>
        private bool HasParameter(int hash)
        {
            foreach (AnimatorControllerParameter parameter in _animator.parameters)
            {
                if (parameter.nameHash == hash)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
