using UnityEngine;
using UnityEngine.InputSystem;

public class GamePauseInput : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
        {
            // 🛑 Buscamos el GameManager real y físico de la escena, sin depender de variables globales
            GameManager manager = FindFirstObjectByType<GameManager>();

            if (manager != null)
            {
                manager.TogglePause();
            }
            else
            {
                Debug.LogError("No se encontró ningún GameManager en la escena para procesar la pausa.");
            }
        }
    }
}
