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
    private void OnDestroy()
    {
        // Limpiamos la memoria al cambiar de nivel
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowPauseMenu(bool isVisible)
    {
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(isVisible);
        }
    }

    // NUEVO MÉTODO: Guarda el estado actual y apaga todo para el viaje de la cámara
    public void PrepareForSettingsTransition()
    {
        if (_mainMenuPanel != null && _mainMenuPanel.activeSelf) _currentActivePanel = _mainMenuPanel;
        if (_pauseMenuPanel != null && _pauseMenuPanel.activeSelf) _currentActivePanel = _pauseMenuPanel;

        _mainMenuPanel?.SetActive(false);
        _pauseMenuPanel?.SetActive(false);
    }

    // Modificado: Solo enciende el panel de ajustes cuando la cámara ya llegó
    public void OpenSettings()
    {
        _settingsMenuPanel?.SetActive(true);
    }

    // Modificado: Apaga ajustes de inmediato al darle "Atrás" para limpiar el viaje de regreso
    public void HideSettingsBeforeTransition()
    {
        _settingsMenuPanel?.SetActive(false);
    }

    // NUEVO MÉTODO: Enciende el menú que correspondía una vez que la cámara regresó
    public void RestorePreviousPanel()
    {
        if (_currentActivePanel != null)
        {
            _currentActivePanel.SetActive(true);
        }
        else if (_mainMenuPanel != null)
        {
            // Salvavidas por si acaso
            _mainMenuPanel.SetActive(true);
        }
    }

    // Conservamos este por si se usa de forma directa en otra parte
    public void CloseSettings()
    {
        _settingsMenuPanel?.SetActive(false);
        RestorePreviousPanel();
    }
}
