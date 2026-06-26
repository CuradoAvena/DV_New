using UnityEngine;

namespace Game.Locomotion
{
    public sealed class WindZone : MonoBehaviour
    {
        [Header("Wind Settings")]
        [SerializeField] private Vector3 _windDirection = Vector3.forward;

        [Tooltip("Prueba con valores entre 2 y 5. Controla qué tan violento es el empuje.")]
        [SerializeField] private float _windForce = 3f;

        private void OnTriggerStay(Collider other)
        {
            var motor = other.GetComponentInParent<CharacterMotor>();
            if (motor != null)
            {
                // Pasamos la dirección normalizada y la fuerza pura.
                // Al bajar la fuerza en el Inspector (ej. a 2 o 3), el CharacterMotor lo sumará
                // de forma uniforme al movimiento planar del input del jugador.
                motor.Move(_windDirection.normalized * _windForce);
            }
        }
    }
}
