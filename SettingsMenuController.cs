using UnityEngine;
using UnityEngine.UI;
using Game.Audio;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public sealed class SettingsMenuController : MonoBehaviour
    {
        [Header("Componentes de UI")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMPro.TMP_Dropdown _qualityDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        private void OnEnable()
        {
            // Sincroniza la UI con los valores reales al encender el panel
            if (_volumeSlider != null && AudioManager.Instance != null)
            {
                _volumeSlider.value = AudioManager.Instance.GetCurrentVolume();
                _volumeSlider.onValueChanged.AddListener(HandleVolumeChanged);
            }

            if (_qualityDropdown != null)
            {
                _qualityDropdown.value = QualitySettings.GetQualityLevel();
                _qualityDropdown.onValueChanged.AddListener(HandleQualityChanged);
            }

            if (_fullscreenToggle != null)
            {
                _fullscreenToggle.isOn = Screen.fullScreen;
                _fullscreenToggle.onValueChanged.AddListener(HandleFullscreenToggled);
            }
        }

        private void OnDisable()
        {
            // Limpieza de listeners al apagar el panel
            if (_volumeSlider != null) _volumeSlider.onValueChanged.RemoveListener(HandleVolumeChanged);
            if (_qualityDropdown != null) _qualityDropdown.onValueChanged.RemoveListener(HandleQualityChanged);
            if (_fullscreenToggle != null) _fullscreenToggle.onValueChanged.RemoveListener(HandleFullscreenToggled);
        }

        private void HandleVolumeChanged(float value)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(value);
        }

        private void HandleQualityChanged(int index)
        {
            QualitySettings.SetQualityLevel(index, true);
        }

        private void HandleFullscreenToggled(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
    }
}