using UnityEngine;
using System.Collections;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public sealed class MenuCameraController : MonoBehaviour
    {
        [Header("Puntos de Cámara")]
        [SerializeField] private Transform _mainMenuPoint;
        [SerializeField] private Transform _settingsMenuPoint;

        [Header("Configuración de Movimiento")]
        [SerializeField] private float _transitionDuration = 1.5f;

        private Coroutine _moveCoroutine;

        private void Start()
        {
            if (_mainMenuPoint != null)
            {
                transform.position = _mainMenuPoint.position;
                transform.rotation = _mainMenuPoint.rotation;
            }
        }

        //  ESTA ES LA FUNCIÓN PARA EL BOTÓN DE AJUSTES
        public void IniciarViajeAAjustes()
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.PrepareForSettingsTransition(); // Apaga el panel principal de inmediato
            }
            StartTransition(_settingsMenuPoint, isGoingToSettings: true);
        }

        // ESTA ES LA FUNCIÓN PARA EL BOTÓN DE REGRESAR
        public void IniciarViajeAlMenuPrincipal()
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.HideSettingsBeforeTransition(); // Apaga los ajustes de inmediato[cite: 3]
            }
            StartTransition(_mainMenuPoint, isGoingToSettings: false);
        }

        private void StartTransition(Transform target, bool isGoingToSettings)
        {
            if (target == null) return;

            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(TransitionRoutine(target, isGoingToSettings));
        }

        private IEnumerator TransitionRoutine(Transform target, bool isGoingToSettings)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float timeElapsed = 0f;

            while (timeElapsed < _transitionDuration)
            {
                timeElapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, timeElapsed / _transitionDuration);

                transform.position = Vector3.Lerp(startPos, target.position, t);
                transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
                yield return null;
            }

            transform.position = target.position;
            transform.rotation = target.rotation;

            // Al llegar, encendemos el panel que corresponda
            if (MenuManager.Instance != null)
            {
                if (isGoingToSettings)
                {
                    MenuManager.Instance.OpenSettings(); // Prende ajustes al llegar[cite: 3]
                }
                else
                {
                    MenuManager.Instance.RestorePreviousPanel(); // Prende el menú principal al regresar[cite: 3]
                }
            }
        }
    }
}