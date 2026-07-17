using UnityEngine;

using UnityEngine.SceneManagement;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public sealed class PauseManager : MonoBehaviour
    {
        [Header("Paneles de UI")]
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _settingsMenuPanel;

        [Header("Referencia al Jugador")]
        // 🚨 AQUÍ: Cambia "MonoBehaviour" por el nombre exacto de tu script de movimiento (ej. ThirdPersonController)
        [SerializeField] private MonoBehaviour _playerController;

        [Header("Configuración de Escenas")]
        [SerializeField] private string _mainMenuSceneName = "Scene_MainMenu";

        private bool _isPaused;

        private void Start()
        {
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
            if (_settingsMenuPanel != null) _settingsMenuPanel.SetActive(false);

            ResumeGame();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (_isPaused)
                {
                    if (_settingsMenuPanel != null && _settingsMenuPanel.activeSelf)
                    {
                        _settingsMenuPanel.SetActive(false);
                        _pauseMenuPanel.SetActive(true);
                    }
                    else
                    {
                        ResumeGame();
                    }
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            _isPaused = true;
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(true);

            Time.timeScale = 0f;

            // 🚨 APAGAMOS EL MOVIMIENTO DEL PERSONAJE
            if (_playerController != null) _playerController.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            _isPaused = false;
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(false);
            if (_settingsMenuPanel != null) _settingsMenuPanel.SetActive(false);

            Time.timeScale = 1f;

            // 🚨 PRENDEMOS EL MOVIMIENTO DEL PERSONAJE DE VUELTA
            if (_playerController != null) _playerController.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}