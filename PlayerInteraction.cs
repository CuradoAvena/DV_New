using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField] private int puntos = 0;

    private PlayerEnergy sistemaEnergia;

    private void Start()
    {
        // Conectamos el script de energía automáticamente al iniciar
        sistemaEnergia = GetComponent<PlayerEnergy>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Usamos CompareTag por limpieza y eficiencia
        if (other.CompareTag("Coleccionable"))
        {
            puntos++;

            // 1. Destruimos el objeto actual del escenario
            Destroy(other.gameObject);

            // 2. Buscamos al gestor usando la función optimizada de Unity 6
            WayPointSpawner spawner = FindFirstObjectByType<WayPointSpawner>();

            if (spawner != null)
            {
                // Le damos la orden de instanciar el siguiente
                spawner.GenerarSiguiente();
            }

            Debug.Log("Objeto recolectado. Puntos: " + puntos);
        }

        else if (other.CompareTag("Bateria"))
        {
            // Buscamos si este cubo específico tiene su propio valor de energía
            ItemEnergia bateriaTocada = other.GetComponent<ItemEnergia>();

            if (bateriaTocada != null && sistemaEnergia != null)
            {
                // Le pasamos la cantidad EXACTA que dice esa batería al jugador
                sistemaEnergia.RecargarEnergia(bateriaTocada.cantidadQueRecarga);

                other.gameObject.SetActive(false); // Apagamos la batería
            }
        }
    }
}
