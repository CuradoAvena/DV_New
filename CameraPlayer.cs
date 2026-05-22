using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    [Header("Configuración de Visión")]
    [SerializeField] private float _sensibilidadMouse = 0.1f;
    [SerializeField] private Transform _cuerpoPadre; // Arrastrar la cápsula aquí

    private float _rotacionX = 0f;
    private Vector2 _inputMirar;

    private void Update()
    {
        LeerMouse();
        RotarMundo();
    }

    private void LeerMouse()
    {
        if (Mouse.current != null)
        {
            _inputMirar = Mouse.current.delta.ReadValue();
        }
    }

    private void RotarMundo()
    {
        // 1. Girar el CUERPO completo de izquierda a derecha (Eje Y)
        float rotacionY = _inputMirar.x * _sensibilidadMouse;
        _cuerpoPadre.Rotate(Vector3.up * rotacionY);

        // 2. Girar SOLO la cámara de arriba a abajo (Eje X) con límites (Clamp)
        _rotacionX -= _inputMirar.y * _sensibilidadMouse;
        _rotacionX = Mathf.Clamp(_rotacionX, -80f, 80f);

        transform.localRotation = Quaternion.Euler(_rotacionX, 0f, 0f);
    }
}
