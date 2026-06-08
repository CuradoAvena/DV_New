using UnityEngine;

public class DañoContacto : MonoBehaviour
{
    [Header("Configuración de Daño")]
[SerializeField] private int puntosDeDaño = 1;

// Usamos OnCollisionEnter porque los enemigos suelen tener cuerpos sólidos
private void OnTriggerEnter(Collider other)
{
    bool esElJugador = other.CompareTag("Player") || other.transform.root.CompareTag("Player");

    if (esElJugador)
    {
        // Buscamos el componente de salud en la raíz del jugador
        PlayerHealth salud = other.transform.root   .GetComponentInChildren<PlayerHealth>();

        if (salud != null)
        {
            salud.RecibirDaño(puntosDeDaño);
        }
    }
}
}
