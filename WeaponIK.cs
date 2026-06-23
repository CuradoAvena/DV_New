using UnityEngine;

namespace Game.Weapons
{
    /// <summary>
    /// Drives left-hand Inverse Kinematics so the character's left hand stays
    /// locked onto the weapon's grip while equipped. Single responsibility: IK.
    /// It reads the <see cref="IWeaponState"/> abstraction and never touches input,
    /// movement or weapon-swapping logic.
    ///
    /// Requirements:
    ///  - Must be on the same GameObject as the <see cref="Animator"/>; Unity only
    ///    calls <see cref="OnAnimatorIK"/> on components next to the Animator.
    ///  - The relevant Animator layer must have "IK Pass" enabled, otherwise
    ///    <see cref="OnAnimatorIK"/> is never invoked.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public sealed class WeaponIK : MonoBehaviour
    {
        [Tooltip("How fast (weight units/second) the hand IK fades in when equipping and out when holstering.")]
        [SerializeField] private float _ikBlendSpeed = 8f;

        private Animator _animator;
        private IWeaponState _weapon;

        // Smoothed 0..1 IK weight so the hand eases onto the grip instead of snapping.
        private float _ikWeight;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _weapon = GetComponentInParent<IWeaponState>();

            if (_weapon == null)
            {
                Debug.LogError(
                    $"{nameof(WeaponIK)} requires a component implementing {nameof(IWeaponState)} " +
                    $"(e.g. {nameof(WeaponController)}) on this object or a parent.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            // Target full weight only when a weapon is equipped AND a grip exists.
            bool active = _weapon != null && _weapon.LeftHandGrip != null;
            float target = active ? 1f : 0f;
            _ikWeight = Mathf.MoveTowards(_ikWeight, target, _ikBlendSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Called by Unity once per Animator layer that has "IK Pass" enabled.
        /// Positions and orients the left hand toward the weapon grip, weighted by
        /// the smoothed <see cref="_ikWeight"/> so it blends cleanly with the
        /// underlying animation.
        /// </summary>
        private void OnAnimatorIK(int layerIndex)
        {
            Transform grip = _weapon != null ? _weapon.LeftHandGrip : null;

            // No target (or fully blended out): release the hand to the animation.
            if (grip == null && _ikWeight <= 0f)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                return;
            }

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);

            if (grip != null)
            {
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, grip.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, grip.rotation);
            }
        }
    }
}
