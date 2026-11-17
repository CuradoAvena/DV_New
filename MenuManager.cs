using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Paneles de UI")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _settingsMenuPanel;

    [Header("Cámara")]
    [SerializeField] private MenuCameraMover _cameraMover;

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
        bool fromMain = _mainMenuPanel != null && _mainMenuPanel.activeSelf;
        if (fromMain) _currentActivePanel = _mainMenuPanel;
        if (_pauseMenuPanel != null && _pauseMenuPanel.activeSelf) _currentActivePanel = _pauseMenuPanel;

        _mainMenuPanel?.SetActive(false);
        _pauseMenuPanel?.SetActive(false);

        // Desde el menú de inicio la cámara viaja al punto B y los ajustes
        // aparecen al llegar. Desde pausa aparecen directo.
        if (fromMain && _cameraMover != null)
            _cameraMover.GoToB(() => _settingsMenuPanel?.SetActive(true));
        else
            _settingsMenuPanel?.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsMenuPanel?.SetActive(false);

        // Regresa al menú de donde venía el usuario
        if (_currentActivePanel == _mainMenuPanel && _cameraMover != null)
        {
            // La cámara vuelve al punto A y el menú aparece al llegar
            _cameraMover.GoToA(() => _currentActivePanel.SetActive(true));
        }
        else if (_currentActivePanel != null)
        {
            _currentActivePanel.SetActive(true);
        }
    }
}