using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Jugador")]
    [SerializeField] private GameObject _playerObject;

    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ojo: destruye SOLO este objeto, no Instance.gameObject
            return;
        }
        Instance = this;
    }

    public void TogglePause() //[cite: 3]
    {
        _isPaused = !_isPaused;

        // 🛑 EL BLINDAJE: Buscamos el MenuManager físico de esta escena exacta, ignorando la memoria global
        MenuManager localMenu = FindFirstObjectByType<MenuManager>();

        if (_isPaused)
        {
            Time.timeScale = 0f; // Congela físicas y animaciones
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            TogglePlayerControls(false);

            if (localMenu != null)
            {
                localMenu.ShowPauseMenu(true);
            }
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el juego
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            TogglePlayerControls(true);

            if (localMenu != null)
            {
                localMenu.ShowPauseMenu(false);
            }
        }
    }

    private void TogglePlayerControls(bool enable) //[cite: 3]
    {
        // 2. ESCUDO: Si al cambiar de escena perdimos al jugador, lo buscamos usando tu controlador
        if (_playerObject == null)
        {
            var playerScript = FindFirstObjectByType<Game.Locomotion.ThirdPersonController>();
            if (playerScript != null) _playerObject = playerScript.gameObject;
        }

        if (_playerObject == null) return; //[cite: 3]

        // Apagamos lector de acciones
        PlayerInput playerInput = _playerObject.GetComponent<PlayerInput>(); //[cite: 3]
        if (playerInput != null) playerInput.enabled = enable; //[cite: 3]

        // Apagamos scripts del jugador (Movimiento)
        MonoBehaviour[] scripts = _playerObject.GetComponents<MonoBehaviour>(); //[cite: 3]
        foreach (MonoBehaviour script in scripts) //[cite: 3]
        {
            if (script != this) script.enabled = enable; //[cite: 3]
        }

        // Apagamos scripts de los hijos (Cámara)
        MonoBehaviour[] childScripts = _playerObject.GetComponentsInChildren<MonoBehaviour>(); //[cite: 3]
        foreach (MonoBehaviour script in childScripts) //[cite: 3]
        {
            if (script.GetType().Name.Contains("Input") || //[cite: 3]
                script.GetType().Name.Contains("Controller") || //[cite: 3]
                script.GetType().Name.Contains("Look")) //[cite: 3]
            {
                script.enabled = enable; //[cite: 3]
            }
        }
    }

    public void LoadScene(string sceneName) //[cite: 3]
    {
        Time.timeScale = 1f; //[cite: 3]
        SceneManager.LoadScene(sceneName); //[cite: 3]
    }
}
