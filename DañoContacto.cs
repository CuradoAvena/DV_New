using UnityEngine;

public class DañoContacto : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int corazonesQueQuita = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si tocamos al jugador
        if (other.CompareTag("Player"))
        {
            // Buscamos el script de salud en el root del jugador
            PlayerHealth saludJugador = other.transform.root.GetComponent<PlayerHealth>();

            if (saludJugador != null)
            {
                saludJugador.RecibirDaño(corazonesQueQuita);
            }
        }
    }
}
