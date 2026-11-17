using System;
using System.Collections;
using UnityEngine;

public class MenuCameraMover : MonoBehaviour
{
    [Header("Puntos")]
    [SerializeField] private Transform _pointA;   // menú de inicio
    [SerializeField] private Transform _pointB;   // ajustes

    [Header("Movimiento")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _routine;

    private void Start()
    {
        // Arranca ya colocada en el punto A
        transform.SetPositionAndRotation(_pointA.position, _pointA.rotation);
    }

    public void GoToA(Action onArrive = null) => MoveTo(_pointA, onArrive);
    public void GoToB(Action onArrive = null) => MoveTo(_pointB, onArrive);

    private void MoveTo(Transform target, Action onArrive)
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(MoveRoutine(target, onArrive));
    }

    private IEnumerator MoveRoutine(Transform target, Action onArrive)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float duration = Mathf.Max(0.01f, _duration);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;   // funciona aunque el juego esté en pausa
            float k = _curve.Evaluate(Mathf.Clamp01(t));
            transform.position = Vector3.Lerp(startPos, target.position,