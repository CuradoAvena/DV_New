using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton simple para acceso global sin acoplamiento
    public static GameManager Instance { get; private set; }

    private bool _isPaused;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        // El truco maestro: congelar el tiempo del motor de Unity
        Time.timeScale = _isPaused ? 0f : 1f;

        // Liberar o bloquear el cursor según el estado
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;

        // Aquí avisamos a la UI de pausa que se prenda o apague
        MenuManager.Instance.ShowPauseMenu(_isPaused);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // Siempre resetear el tiempo al cambiar de escena
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
