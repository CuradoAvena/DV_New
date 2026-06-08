using UnityEngine;

public class EnemigoPatrulla : MonoBehaviour
{
    [Header("Ruta de Patrullaje")]
    [Tooltip("Arrastra aquí los Empty GameObjects que servirán de puntos de ruta")]
    [SerializeField] private Transform[] puntosRuta;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float velocidadRotacion = 10f; // Qué tan rápido gira su cuerpo

    private int indiceActual = 0;

    private void Update()
    {
        if (puntosRuta.Length == 0) return;

        Transform objetivo = puntosRuta[indiceActual];

        // --- NUEVO: Lógica de Rotación ---
        // 1. Calculamos el vector de dirección (Destino menos Origen)
        Vector3 direccion = objetivo.position - transform.position;

        // 2. Bloqueamos el eje Y para que el enemigo no se incline hacia el piso o el techo 
        // si el punto de ruta está más alto o más bajo
        direccion.y = 0;

        // 3. Si la dirección no es cero, rotamos suavemente hacia allá
        if (direccion.magnitude > 0.1f)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }
        // ---------------------------------

        // Lógica de Movimiento (La que ya teníamos)
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);

        // Comprobación de llegada
        if (Vector3.Distance(transform.position, objetivo.position) < 0.1f)
        {
            indiceActual++;
            if (indiceActual >= puntosRuta.Length)
            {
                indiceActual = 0;
            }
        }
    }
}
