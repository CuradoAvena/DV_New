using UnityEngine;

namespace Game.Locomotion
{
    /// <summary>
    /// Abstraction over any input source that can drive a character.
    /// Movement/physics code depends on this interface, never on a concrete
    /// input backend (Dependency Inversion Principle). This lets us swap the
    /// player's keyboard/gamepad reader for an AI brain, a replay system or a
    /// network proxy without touching the locomotion code.
    /// </summary>
    public interface ICharacterInput
    {
        /// <summary>Normalized planar movement intent. X = strafe, Y = forward/back. Range [-1, 1].</summary>
        Vector2 MoveInput { get; }

        /// <summary>True while the sprint control is held down.</summary>
        bool SprintHeld { get; }

        /// <summary>True only on the single frame the jump control is pressed.</summary>
        bool JumpRequested { get; }

        /// <summary>True only on the single frame the equip/holster toggle is pressed.</summary>
        bool EquipToggleRequested { get; }
    }
}
