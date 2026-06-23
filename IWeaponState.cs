using System;
using UnityEngine;

namespace Game.Weapons
{
    /// <summary>
    /// Read-only view of the character's weapon state, published by the
    /// <see cref="WeaponController"/> and consumed by other systems
    /// (movement speed clamping, hand IK, HUD, audio, ...).
    ///
    /// This is the decoupling seam of the weapon feature: consumers depend on
    /// this abstraction rather than on the concrete controller (Dependency
    /// Inversion Principle), so movement and IK never need to know how weapons
    /// are actually swapped or which input triggers them.
    /// </summary>
    public interface IWeaponState
    {
        /// <summary>True while the weapon is drawn in the hands (aiming pose active).</summary>
        bool IsEquipped { get; }

        /// <summary>
        /// World-space transform on the equipped weapon's barrel that the left hand
        /// should grip. Null while holstered (or if no grip was assigned), which the
        /// IK layer treats as "no target".
        /// </summary>
        Transform LeftHandGrip { get; }

        /// <summary>Raised whenever the equipped/holstered state changes.</summary>
        event Action EquipChanged;
    }
}
