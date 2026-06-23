using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Locomotion
{
    /// <summary>
    /// Reads player input through Unity's New Input System and exposes it through
    /// the <see cref="ICharacterInput"/> abstraction.
    ///
    /// This is the ONLY class in the locomotion stack that references the Input
    /// System (Single Responsibility Principle). The actions are created in code
    /// so the controller is fully self-contained and works without any external
    /// .inputactions asset wiring. Supports Keyboard (WASD / Arrows) and Gamepad.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerInputReader : MonoBehaviour, ICharacterInput
    {
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _jumpAction;
        private InputAction _equipAction;

        /// <inheritdoc />
        public Vector2 MoveInput => _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;

        /// <inheritdoc />
        public bool SprintHeld => _sprintAction != null && _sprintAction.IsPressed();

        /// <inheritdoc />
        public bool JumpRequested => _jumpAction != null && _jumpAction.WasPressedThisFrame();

        /// <inheritdoc />
        public bool EquipToggleRequested => _equipAction != null && _equipAction.WasPressedThisFrame();

        /// <summary>
        /// Builds the input actions and their bindings. Done once on Awake so the
        /// component is ready before any consumer reads from it.
        /// </summary>
        private void Awake()
        {
            // --- Move: 2D vector from WASD, Arrow keys, or the gamepad left stick ---
            _moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            _moveAction.AddBinding("<Gamepad>/leftStick");

            // --- Sprint: held button ---
            _sprintAction = new InputAction("Sprint", InputActionType.Button);
            _sprintAction.AddBinding("<Keyboard>/leftShift");
            _sprintAction.AddBinding("<Gamepad>/leftStickPress");

            // --- Jump: discrete press ---
            _jumpAction = new InputAction("Jump", InputActionType.Button);
            _jumpAction.AddBinding("<Keyboard>/space");
            _jumpAction.AddBinding("<Gamepad>/buttonSouth");

            // --- Equip/Holster: discrete toggle press ---
            _equipAction = new InputAction("Equip", InputActionType.Button);
            _equipAction.AddBinding("<Keyboard>/f");
            _equipAction.AddBinding("<Gamepad>/buttonNorth");
        }

        /// <summary>Enable actions only while the component is active.</summary>
        private void OnEnable()
        {
            _moveAction?.Enable();
            _sprintAction?.Enable();
            _jumpAction?.Enable();
            _equipAction?.Enable();
        }

        /// <summary>Disable actions when the component is deactivated.</summary>
        private void OnDisable()
        {
            _moveAction?.Disable();
            _sprintAction?.Disable();
            _jumpAction?.Disable();
            _equipAction?.Disable();
        }

        /// <summary>Release the unmanaged input action resources.</summary>
        private void OnDestroy()
        {
            _moveAction?.Dispose();
            _sprintAction?.Dispose();
            _jumpAction?.Dispose();
            _equipAction?.Dispose();
        }
    }
}
