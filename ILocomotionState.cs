using System;

namespace Game.Locomotion
{
    /// <summary>
    /// Read-only view of a character's locomotion state, published by the movement
    /// layer and consumed by the animation layer.
    ///
    /// This is the seam that keeps movement and animation decoupled: the movement
    /// controller never references the Animator, and the animation controller never
    /// touches the CharacterController. The animator depends only on this
    /// abstraction (Dependency Inversion Principle), so movement can be swapped or
    /// tested without affecting animation, and vice-versa.
    /// </summary>
    public interface ILocomotionState
    {
        /// <summary>
        /// Current planar speed remapped to the 0..1 range (1 = full sprint speed).
        /// Designed to feed a 1D locomotion Blend Tree's "Speed" parameter directly.
        /// </summary>
        float NormalizedSpeed { get; }

        /// <summary>True while the character is standing on the ground this frame.</summary>
        bool IsGrounded { get; }

        /// <summary>
        /// Raised the instant a jump is initiated from the ground. The animation
        /// layer subscribes to this to fire the Animator's "Jump" trigger exactly
        /// once per jump, rather than polling every frame.
        /// </summary>
        event Action Jumped;
    }
}
