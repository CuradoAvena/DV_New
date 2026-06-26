using UnityEngine;

namespace Game.Locomotion
{
    public sealed class LowGravityZone : EnvironmentZone
    {
        [Tooltip("0.1 significa el 10% de la gravedad normal (flotar mucho)")]
        [SerializeField][Range(0.05f, 1f)] private float _gravityMultiplier = 0.2f;

        // Cambiamos el comportamiento base para que asegure el valor frame a frame mientras esté dentro
        private void OnTriggerStay(Collider other)
        {
            var motor = other.GetComponentInParent<CharacterMotor>();
            if (motor != null)
            {
                motor.SetGravityModifier(_gravityMultiplier);
            }
        }

        protected override void OnCharacterEnter(ThirdPersonController controller, CharacterMotor motor)
        {
            motor.SetGravityModifier(_gravityMultiplier);
        }

        protected override void OnCharacterExit(ThirdPersonController controller, CharacterMotor motor)
        {
            motor.SetGravityModifier(1f); // Al salir completamente, se limpia seguro
        }
    }
}