using UnityEngine;

public class WayPointSpawner : MonoBehaviour
{
    [Header("Configuración de Coleccionables")]
    // Aquí arrastran el Prefab del modelo que hicieron en Maya
    [SerializeField] private GameObject prefabColeccionable;

    // Arreglo para colocar los objetos vacíos que servirán de ruta
    [SerializeField] private Transform[] waypoints;

    private int indiceActual = 0;

    void Start()
    {
        // Instanciamos el primer objeto al iniciar el nivel
        GenerarSiguiente();
    }

    public void GenerarSiguiente()
    {
        // Verificamos que aún nos queden puntos en la lista
        if (indiceActual < waypoints.Length)
        {
            // Aparece el objeto en la posición del waypoint actual
            Instantiate(prefabColeccionable, waypoints[indiceActual].position, Quaternion.identity);

            // Avanzamos al siguiente índice para la próxima vez
            indiceActual++;
        }
        else
        {
            Debug.Log("¡Ruta completada! No hay más waypoints.");
        }
    }   
}
