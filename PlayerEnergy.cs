using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Configuración de Energía")]
    public float energiaMaxima = 100f;
    public float energiaActual; // Lo dejamos público para que el FPController lo pueda leer

    private void Start()
    {
        energiaActual = energiaMaxima;
    }

    public void RecargarEnergia(float cantidad)
    {
        energiaActual += cantidad;
        energiaActual = Mathf.Clamp(energiaActual, 0, energiaMaxima);
        Debug.Log("Recarga. Energía: " + energiaActual);
    }

    // Nuevo: Consumir energía continuamente
    public void GastarEnergia(float cantidad)
    {
        energiaActual -= cantidad;
        energiaActual = Mathf.Clamp(energiaActual, 0, energiaMaxima);
    }

    // Nuevo: Una pregunta sencilla que nos responde con Verdadero o Falso
    public bool TieneEnergia()
    {
        return energiaActual > 0;
    }
}
