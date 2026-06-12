using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{    // Clean Code: Patrón Singleton para acceso global rápido
    public static CombatManager Instancia { get; private set; }

    [Header("Control de Flancos")]
    public string ocupanteDerecha = "";
    public string ocupanteIzquierda = "";
    public string ocupanteFrente = "";

    // Cola de enemigos que no consiguieron posición y están esperando su turno
    // Usamos Queue porque el primero en llegar debe ser el primero en entrar (FIFO)
    private Queue<EnemigoTactico> colaDeEspera = new Queue<EnemigoTactico>();

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Los enemigos llaman a esta función cuando detectan al jugador.
    // Ahora recibe la referencia al enemigo completo, no solo su nombre,
    // para poder avisarle cuando se libere un hueco.
    public string SolicitarPosicionEstrategica(EnemigoTactico solicitante)
    {
        if (ocupanteDerecha == "")
        {
            ocupanteDerecha = solicitante.MiNombre;
            return "Derecha";
        }
        else if (ocupanteIzquierda == "")
        {
            ocupanteIzquierda = solicitante.MiNombre;
            return "Izquierda";
        }
        else if (ocupanteFrente == "")
        {
            ocupanteFrente = solicitante.MiNombre;
            return "Frente";
        }

        // No hay hueco: lo ponemos en la cola si aún no está en ella
        if (!colaDeEspera.Contains(solicitante))
        {
            colaDeEspera.Enqueue(solicitante);
            Debug.Log($"{solicitante.MiNombre}: sin posición libre, esperando turno ({colaDeEspera.Count} en cola).");
        }

        return "Espera";
    }

    // Es vital que los enemigos llamen a esto si mueren o si pierden al jugador,
    // para liberar el espacio y avisar al siguiente en la cola.
    public void LiberarPosicion(string posicion)
    {
        if (posicion == "Derecha") ocupanteDerecha = "";
        if (posicion == "Izquierda") ocupanteIzquierda = "";
        if (posicion == "Frente") ocupanteFrente = "";

        // Si hay alguien esperando, le cedemos el hueco que acaba de quedar libre
        IntentarAsignarSiguienteEnCola();
    }

    // Recorre la cola hasta encontrar un enemigo que siga vivo y activo
    private void IntentarAsignarSiguienteEnCola()
    {
        while (colaDeEspera.Count > 0)
        {
            EnemigoTactico siguiente = colaDeEspera.Dequeue();

            // Puede que el enemigo haya muerto mientras esperaba; lo saltamos
            if (siguiente == null) continue;

            // Le pedimos que solicite posición de nuevo; ahora sí habrá un hueco
            string nuevaPos = SolicitarPosicionEstrategica(siguiente);

            if (nuevaPos != "Espera")
            {
                // Le notificamos directamente para que salga del estado de espera
                siguiente.RecibirAsignacionDesdeCola(nuevaPos);
                Debug.Log($"{siguiente.MiNombre}: salió de la cola, asignado a {nuevaPos}.");
                return; // Solo asignamos un hueco por vez
            }
        }
    }

    // Permite que un enemigo se retire de la cola voluntariamente
    // (por ejemplo, si pierde al jugador de vista mientras esperaba)
    public void SalirDeCola(EnemigoTactico enemigo)
    {
        // Queue no tiene Remove directo; reconstruimos sin ese elemento
        Queue<EnemigoTactico> colaFiltrada = new Queue<EnemigoTactico>();
        foreach (EnemigoTactico e in colaDeEspera)
        {
            if (e != enemigo) colaFiltrada.Enqueue(e);
        }
        colaDeEspera = colaFiltrada;
    }

}
