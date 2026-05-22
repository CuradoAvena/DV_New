using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Controles (Nuevo Input System)")]
    // Definimos las acciones para que las configuren en el Inspector
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;


    [Header("Configuración de Físicas")]
    [SerializeField] private float gravity = -9.81f;

    // El Transform y el LayerMask necesitan ser asignados desde el Inspector sí o sí
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    // Estas variables son 100% internas de la lógica, no necesitan verse en el Inspector
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {// 1. Verificación de Suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Leer los Inputs del Nuevo Sistema
        // Nos devuelve un Vector2 (X, Y), que usaremos para movernos hacia los lados y hacia adelante
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // 3. Aplicar Movimiento Horizontal
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        // 4. Lógica de Salto
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Aplicar Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}