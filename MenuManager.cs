using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Paneles de UI")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _settingsMenuPanel;

    private GameObject _currentActivePanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPauseMenu(bool isVisible)
    {
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(isVisible);
        }
    }

    public void OpenSettings()
    {
        // Guarda temporalmente cuál panel estaba abierto para poder regresar
        if (_mainMenuPanel != null && _mainMenuPanel.activeSelf) _currentActivePanel = _mainMenuPanel;
        if (_pauseMenuPanel != null && _pauseMenuPanel.activeSelf) _currentActivePanel = _pauseMenuPanel;

        _mainMenuPanel?.SetActive(false);
        _pauseMenuPanel?.SetActive(false);
        _settingsMenuPanel?.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsMenuPanel?.SetActive(false);
        // Regresa al menú de donde venía el usuario
        if (_currentActivePanel != null)
        {
            _currentActivePanel.SetActive(true);
        }
    }
}
