using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Indispensable para controlar los corazones
using System.Collections;
public class PlayerHealth : MonoBehaviour
{

    [Header("Sistema de Vidas")]
    public int vidasMaximas = 3;
    private int vidasActuales;

    [Header("Interfaz de Corazones")]
    [Tooltip("Arrastra aquí las imágenes de los corazones de tu Canvas")]
    public Image[] iconosCorazones;
    private void Start()
    {
        // Al iniciar, llenamos el tanque de salud
        vidasActuales = vidasMaximas;
        ActualizarCorazones();
    }

    public void RecibirDaño(int cantidadCorazones)
    {
        // Guardamos cuánta vida teníamos antes del golpe
        int vidaAnterior = vidasActuales;

        vidasActuales -= cantidadCorazones;
        if (vidasActuales < 0) vidasActuales = 0;

        Debug.Log($"¡Ouch! Me quedan {vidasActuales} corazones.");

        // EL INTERRUPTOR: Aquí llamamos a tu corrutina para los corazones que perdimos
        for (int i = vidaAnterior - 1; i >= vidasActuales; i--)
        {
            if (i >= 0 && i < iconosCorazones.Length)
            {
                // ¡Esta es la línea que enciende la animación!
                StartCoroutine(ParpadearYApagar(iconosCorazones[i]));
            }
        }

        if (vidasActuales == 0)
        {
            Morir();
        }
    }

    private void ActualizarCorazones()
    {
        // Recorremos nuestra lista de imágenes de corazones en la UI
        for (int i = 0; i < iconosCorazones.Length; i++)
        {
            // Si el número de corazón es menor a mis vidas, se enciende. Si no, se apaga.
            if (i < vidasActuales)
            {
                iconosCorazones[i].enabled = true; // Corazón lleno
            }
            else
            {
                iconosCorazones[i].enabled = false; // Corazón vacío/apagado
            }
        }
    }

    private IEnumerator ParpadearYApagar(Image corazon)
    {
        // El corazón parpadea 3 veces rápido (0.1 segundos por cambio)
        for (int i = 0; i < 3; i++)
        {
            corazon.enabled = false;
            yield return new WaitForSeconds(0.1f);
            corazon.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        // Se apaga definitivamente después del drama
        corazon.enabled = false;
    }
    private void Morir()
    {
        Debug.Log("¡El mago ha caído! Reiniciando nivel...");
        // 2. En lugar de recargar de golpe, llamamos a la corrutina
        StartCoroutine(RecargarEscenaLimpia());
    }

    // 3. Esta es la magia que salva a la consola de los fantasmas
    private IEnumerator RecargarEscenaLimpia()
    {
        // Esperamos a que termine el frame actual antes de destruir los objetos
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
