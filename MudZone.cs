using UnityEngine;

namespace Game.Locomotion
{
    public sealed class MudZone : EnvironmentZone
    {
        [SerializeField][Range(0.1f, 0.8f)] private float _mudSpeedMultiplier = 0.3f;

        protected override void OnCharacterEnter(ThirdPersonController controller, CharacterMotor motor)
        {
            controller.SetSpeedMultiplier(_mudSpeedMultiplier);
        }

        protected override void OnCharacterExit(ThirdPersonController controller, CharacterMotor motor)
        {
            controller.SetSpeedMultiplier(1f); // Restaura velocidad normal
        }
    }
}