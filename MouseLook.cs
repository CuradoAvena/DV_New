using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    [SerializeField] private float mouseSensitivity = 50f;

    // Asignamos valores por defecto razonables (-90 y 90 grados es el estándar para no "rompernos el cuello")
    [SerializeField] private float clampNeg = -90f;
    [SerializeField] private float clampPos = 90f;

    [Header("Referencias")]
    // Aquí arrastrarán el objeto principal del jugador (el que tiene el CharacterController)
    [SerializeField] private Transform playerBody;

    [Header("Controles (Nuevo Input System)")]
    // Usamos InputAction para leer el movimiento del mouse
    [SerializeField] private InputAction lookAction;

    private float xRotation = 0f;

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    void Start()
    {
        // ¡Súper útil para los alumnos! Oculta el cursor y lo bloquea en el centro del juego.
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Leer el movimiento del mouse (nos devuelve un Vector2 con el desplazamiento X y Y)
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // 2. Multiplicar por la sensibilidad y el tiempo
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // 3. Calcular la rotación vertical (mirar arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, clampNeg, clampPos);

        // 4. Aplicar la rotación a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 5. Rotar todo el cuerpo del jugador horizontalmente (izquierda/derecha)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
