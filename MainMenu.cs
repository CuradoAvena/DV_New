using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void CargarJuego(string nombreEscena)
    {
        // Nos aseguramos de que el tiempo esté corriendo al cargar el nivel
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}
