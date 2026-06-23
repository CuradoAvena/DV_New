using System;
using UnityEngine;
using Game.Locomotion;

namespace Game.Weapons
{
    /// <summary>
    /// Handles equipping/holstering a two-handed weapon. Single responsibility:
    /// translate an equip input into (a) a visual weapon swap between two sockets
    /// and (b) an Upper Body Animator layer blend. It owns no movement or IK
    /// logic — those systems simply read the <see cref="IWeaponState"/> it
    /// publishes.
    ///
    /// Must live on the same GameObject as the character's <see cref="Animator"/>
    /// (so it can drive the Upper Body layer weight). The two weapon visuals are
    /// expected to be pre-parented in the scene:
    ///  - <see cref="_holsteredWeapon"/> under a Spine socket (shown when holstered)
    ///  - <see cref="_equippedWeapon"/> under a Right Hand socket (shown when equipped)
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public sealed class WeaponController : MonoBehaviour, IWeaponState
    {
        [Header("Weapon Visuals")]
        [Tooltip("Weapon object parented to the Spine socket, visible while holstered.")]
        [SerializeField] private GameObject _holsteredWeapon;

        [Tooltip("Weapon object parented to the Right Hand socket, visible while equipped.")]
        [SerializeField] private GameObject _equippedWeapon;

        [Header("Left Hand IK")]
        [Tooltip("Grip transform on the EQUIPPED weapon's barrel that the left hand snaps to.")]
        [SerializeField] private Transform _leftHandGrip;

        [Header("Upper Body Animator Layer")]
        [Tooltip("Name of the masked Animator layer that plays the aiming poses.")]
        [SerializeField] private string _upperBodyLayerName = "Upper Body";

        [Tooltip("How fast (weight units/second) the upper body layer blends in and out.")]
        [SerializeField] private float _layerBlendSpeed = 6f;

        private Animator _animator;
        private ICharacterInput _input;
        private int _upperBodyLayerIndex = -1;
        private bool _isEquipped;
        private float _targetLayerWeight;

        // --- IWeaponState ---

        /// <inheritdoc />
        public bool IsEquipped => _isEquipped;

        /// <inheritdoc />
        public Transform LeftHandGrip => _isEquipped ? _leftHandGrip : null;

        /// <inheritdoc />
        public event Action EquipChanged;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            // Input is resolved through the abstraction; it may sit on this object
            // or a parent (covers rigs where scripts and Animator differ in depth).
            _input = GetComponentInParent<ICharacterInput>();

            _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);
            if (_upperBodyLayerIndex < 0)
            {
                Debug.LogWarning(
                    $"{nameof(WeaponController)}: Animator has no layer named '{_upperBodyLayerName}'. " +
                    "The aiming pose will not blend in.", this);
            }

            // Start holstered and make the visuals match immediately.
            ApplyVisualState();
        }

        private void Update()
        {
            if (_input != null && _input.EquipToggleRequested)
            {
                Toggle();
            }

            BlendUpperBodyLayer();
        }

        /// <summary>Flips between equipped and holstered.</summary>
        public void Toggle() => SetEquipped(!_isEquipped);

        /// <summary>
        /// Sets the equipped state explicitly (useful for cutscenes, pickups, etc.).
        /// No-ops if already in the requested state so the event only fires on change.
        /// </summary>
        public void SetEquipped(bool equipped)
        {
            if (_isEquipped == equipped)
            {
                return;
            }

            _isEquipped = equipped;
            ApplyVisualState();
            EquipChanged?.Invoke();
        }

        /// <summary>
        /// Toggles the two weapon GameObjects and sets the target weight for the
        /// upper body layer. The actual layer weight is eased toward this target in
        /// <see cref="BlendUpperBodyLayer"/> for a smooth draw/holster.
        /// </summary>
        private void ApplyVisualState()
        {
            if (_equippedWeapon != null)
            {
                _equippedWeapon.SetActive(_isEquipped);
            }

            if (_holsteredWeapon != null)
            {
                _holsteredWeapon.SetActive(!_isEquipped);
            }

            _targetLayerWeight = _isEquipped ? 1f : 0f;
        }

        /// <summary>Eases the Upper Body layer weight toward its target each frame.</summary>
        private void BlendUpperBodyLayer()
        {
            if (_upperBodyLayerIndex < 0)
            {
                return;
            }

            float current = _animator.GetLayerWeight(_upperBodyLayerIndex);
            float next = Mathf.MoveTowards(current, _targetLayerWeight, _layerBlendSpeed * Time.deltaTime);
            _animator.SetLayerWeight(_upperBodyLayerIndex, next);
        }
    }
}
