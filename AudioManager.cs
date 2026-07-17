using UnityEngine;

namespace Game.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Configuración Inicial")]
        [Range(0f, 1f)][SerializeField] private float _defaultVolume = 0.5f;

        private float _currentVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que Unity borre el objeto al cambiar de escena

            SetVolume(_defaultVolume);
        }

        public void SetVolume(float volume)
        {
            _currentVolume = Mathf.Clamp01(volume);
            AudioListener.volume = _currentVolume; // Cambia el volumen maestro de Unity
        }

        public float GetCurrentVolume() => _currentVolume;
    }
}