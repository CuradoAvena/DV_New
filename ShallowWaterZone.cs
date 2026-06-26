using UnityEngine;


   namespace Game.Locomotion
    {
        public sealed class ShallowWaterZone : EnvironmentZone
        {
            [SerializeField] private float _waterSpeedMultiplier = 0.6f;
            // Si en el futuro expones el JumpHeight en el TPC, aquí podrías reducirlo a la mitad

            protected override void OnCharacterEnter(ThirdPersonController controller, CharacterMotor motor)
            {
                controller.SetSpeedMultiplier(_waterSpeedMultiplier);
            }

            protected override void OnCharacterExit(ThirdPersonController controller, CharacterMotor motor)
            {
                controller.SetSpeedMultiplier(1f);
            }
        }
    }
