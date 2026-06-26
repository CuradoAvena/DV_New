using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Locomotion
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target Setup")]
        [Tooltip("El personaje al que la cámara va a seguir.")]
        [SerializeField] private Transform _target;

        [Tooltip("Distancia entre la cámara y el personaje.")]
        [SerializeField] private float _distance = 5f;

        [Tooltip("Desfase de altura para no apuntar directamente a los pies.")]
        [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Sensitivity & Limits")]
        [SerializeField] private float _sensitivityX = 0.2f;
        [SerializeField] private float _sensitivityY = 0.2f;
        [SerializeField] private float _minVerticalAngle = -20f;
        [SerializeField] private float _maxVerticalAngle = 60f;

        private InputAction _lookAction;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private void Awake()
        {
            // Inicializamos la lectura del mouse usando el New Input System vía código
            _lookAction = new InputAction("Look", InputActionType.Value, expectedControlType: "Vector2");
            _lookAction.AddBinding("<Mouse>/delta");
            _lookAction.AddBinding("<Gamepad>/rightStick");
        }

        private void OnEnable() => _lookAction?.Enable();
        private void OnDisable() => _lookAction?.Disable();
        private void OnDestroy() => _lookAction?.Dispose();

        private void Start()
        {
            // Bloquea el cursor en el centro de la pantalla para comodidad en tercera persona
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector2 lookInput = _lookAction.ReadValue<Vector2>();

            // Acumulamos los valores del mouse multiplicados por la sensibilidad
            _cinemachineTargetYaw += lookInput.x * _sensitivityX;
            _cinemachineTargetPitch -= lookInput.y * _sensitivityY;

            // Clampeamos la rotación vertical para evitar que la cámara de la vuelta completa
            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, _minVerticalAngle, _maxVerticalAngle);

            // Calculamos la rotación y la posición final de la cámara rodeando al target
            Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
            Vector3 targetPosition = _target.position + _targetOffset;

            transform.rotation = targetRotation;
            transform.position = targetPosition - (targetRotation * Vector3.forward * _distance);
        }
    }
}