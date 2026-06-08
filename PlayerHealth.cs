using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaActual;

    private void Start()
    {
        // Al iniciar, llenamos el tanque de salud
        vidaActual = vidaMaxima;
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("¡Auch! Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("¡El jugador ha caído! Reiniciando nivel...");
        // Recarga la escena actual tal como lo hace nuestra trampa del vacío
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
