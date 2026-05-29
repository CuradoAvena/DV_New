using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float sprintSpeed = 10f; //linea para correr

    [SerializeField] private float costoEnergiaPorSegundo = 15f; //energia que gasta

    [Header("Controles (Nuevo Input System)")]
    // Definimos las acciones para que las configuren en el Inspector
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private InputAction sprintAction; // NUEVO: Acción para correr


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
    private PlayerEnergy sistemaEnergia; // Referencia a nuestra batería

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        sistemaEnergia = GetComponent<PlayerEnergy>(); //energia
    }

    void Update()
    {// 1. Verificación de Suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float velocidadActual = speed;
        // Verificamos si presiona el botón y si tiene batería
        if (sprintAction.IsPressed() && sistemaEnergia != null && sistemaEnergia.TieneEnergia())
        {
            velocidadActual = sprintSpeed; // Aceleramos
            sistemaEnergia.GastarEnergia(costoEnergiaPorSegundo * Time.deltaTime);
        }

        // 3. Leer los Inputs (¡Solo lo declaramos una vez aquí!)
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // 4. Aplicar Movimiento Horizontal con la velocidad dinámica
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * velocidadActual * Time.deltaTime);

        // 5. Lógica de Salto
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 6. Aplicar Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}