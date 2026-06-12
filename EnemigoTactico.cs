using UnityEngine;
using UnityEngine.AI;

public class EnemigoTactico : MonoBehaviour
{
    // Añadimos "Esperando" como estado explícito para los que están en cola
    public enum EstadoIA { Patrullando, Esperando, Posicionandose, Atacando }

    [Header("Estado Actual")]
    [SerializeField] private EstadoIA estadoActual = EstadoIA.Patrullando;
    [SerializeField] private string miRolTactico = "";

    [Header("Narrativa y Colaboración")]
    [SerializeField] private string miNombre = "Guardia";

    // Propiedad pública de solo lectura para que CombatManager pueda leer el nombre
    public string MiNombre => miNombre;

    [Header("Configuración de IA")]
    [SerializeField] private float rangoVision = 12f;
    [SerializeField] private float rangoAtaque = 2.5f;
    [SerializeField] private float distanciaFlanqueo = 3f;

    [Header("Visión (Raycast)")]
    [Tooltip("Capa(s) que bloquean la visión (paredes, obstáculos). Asigna 'Obstaculos' o similar.")]
    [SerializeField] private LayerMask capasObstaculo;
    [Tooltip("Altura desde la que sale el rayo de visión (aprox. altura de los ojos)")]
    [SerializeField] private float alturaOjos = 1.6f;

    [Header("Patrullaje Libre (Wander)")]
    [SerializeField] private float radioPatrullaje = 10f;
    [SerializeField] private float tiempoEntrePaseos = 2f;
    private float temporizadorPatrulla = 0f;

    private Vector3 posicionInicial;
    private Transform jugador;
    private NavMeshAgent agente;
    private MeshRenderer renderCuerpo;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        renderCuerpo = GetComponent<MeshRenderer>();
        posicionInicial = transform.position;

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;
    }

    private void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        switch (estadoActual)
        {
            case EstadoIA.Patrullando:
                ComportamientoPatrulla(distanciaAlJugador);
                break;
            case EstadoIA.Esperando:
                ComportamientoEspera(distanciaAlJugador);
                break;
            case EstadoIA.Posicionandose:
                ComportamientoPosicionamiento(distanciaAlJugador);
                break;
            case EstadoIA.Atacando:
                ComportamientoAtaque(distanciaAlJugador);
                break;
        }
    }

    /// <summary>
    /// Devuelve true solo si el jugador está dentro del rango Y sin obstáculos entre medio.
    /// El rayo sale desde la altura de los ojos del enemigo hacia la altura de los ojos del jugador.
    /// </summary>
    private bool TengoLineaDeVisionAlJugador(float distancia)
    {
        if (distancia > rangoVision) return false;

        Vector3 origen = transform.position + Vector3.up * alturaOjos;
        Vector3 destino = jugador.position + Vector3.up * alturaOjos;
        Vector3 dir = destino - origen;

        // Si el rayo choca con algo en las capas de obstáculo antes de llegar al jugador,
        // la línea de visión está bloqueada.
        if (Physics.Raycast(origen, dir.normalized, out RaycastHit hit, distancia, capasObstaculo))
        {
            // Dibujamos en rojo cuando hay obstáculo (visible en Scene view con Gizmos activos)
            Debug.DrawRay(origen, dir, Color.red);
            return false;
        }

        // Sin obstáculo: línea de visión limpia
        Debug.DrawRay(origen, dir, Color.green);
        return true;
    }

    private void ComportamientoPatrulla(float distancia)
    {
        agente.speed = 2f;
        renderCuerpo.material.color = Color.white;

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            temporizadorPatrulla += Time.deltaTime;
            if (temporizadorPatrulla >= tiempoEntrePaseos)
            {
                BuscarNuevoPuntoAleatorio();
                temporizadorPatrulla = 0f;
            }
        }

        // CAMBIO CLAVE: ahora usamos TengoLineaDeVisionAlJugador en lugar de solo distancia
        if (TengoLineaDeVisionAlJugador(distancia))
        {
            miRolTactico = CombatManager.Instancia.SolicitarPosicionEstrategica(this);

            if (miRolTactico == "Espera")
            {
                // No hay hueco: pasamos a esperar sin movernos encima del jugador
                estadoActual = EstadoIA.Esperando;
                agente.isStopped = true;
            }
            else
            {
                estadoActual = EstadoIA.Posicionandose;
                GritarOrdenTactica();
            }
        }
    }

    /// <summary>
    /// El enemigo está en cola: se queda quieto, color azul, esperando que CombatManager
    /// lo llame con RecibirAsignacionDesdeCola(). Si pierde al jugador de vista, se retira de la cola.
    /// </summary>
    private void ComportamientoEspera(float distancia)
    {
        renderCuerpo.material.color = Color.blue; // Azul: en espera de turno

        // Si el jugador sale del rango de visión, se retira de la cola y vuelve a patrullar
        if (!TengoLineaDeVisionAlJugador(distancia))
        {
            CombatManager.Instancia.SalirDeCola(this);
            miRolTactico = "";
            agente.isStopped = false;
            estadoActual = EstadoIA.Patrullando;
            BuscarNuevoPuntoAleatorio();
            Debug.Log($"{miNombre}: perdió al jugador de vista, saliendo de la cola.");
        }
    }

    private void ComportamientoPosicionamiento(float distancia)
    {
        agente.speed = 4.5f;
        renderCuerpo.material.color = new Color(1f, 0.5f, 0f);

        Vector3 destinoTeorico = CalcularDestinoPorRol();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destinoTeorico, out hit, 2f, NavMesh.AllAreas))
            agente.SetDestination(hit.position);
        else
            agente.SetDestination(jugador.position);

        // Perdió línea de visión O el jugador escapó muy lejos → vuelve a patrullar
        if (!TengoLineaDeVisionAlJugador(distancia) || distancia > rangoVision * 1.5f)
        {
            LiberarMiRol();
            agente.isStopped = false;
            estadoActual = EstadoIA.Patrullando;
            BuscarNuevoPuntoAleatorio();
        }
        else if (distancia <= rangoAtaque)
        {
            estadoActual = EstadoIA.Atacando;
        }
    }

    private void ComportamientoAtaque(float distancia)
    {
        agente.isStopped = true;
        renderCuerpo.material.color = Color.red;

        Debug.Log($"¡{miNombre} atacando desde la {miRolTactico}!");

        // Pierde al jugador de vista estando en rango de ataque (p. ej. el jugador esquiva detrás de una columna)
        if (!TengoLineaDeVisionAlJugador(distancia))
        {
            agente.isStopped = false;
            estadoActual = EstadoIA.Posicionandose; // Intenta volver a flanquear
        }
        else if (distancia > rangoAtaque)
        {
            agente.isStopped = false;
            estadoActual = EstadoIA.Posicionandose;
        }
    }

    /// <summary>
    /// CombatManager llama a este método cuando se libera un hueco y le toca a este enemigo.
    /// </summary>
    public void RecibirAsignacionDesdeCola(string nuevaPos)
    {
        miRolTactico = nuevaPos;
        agente.isStopped = false;
        estadoActual = EstadoIA.Posicionandose;
        GritarOrdenTactica();
    }

    private Vector3 CalcularDestinoPorRol()
    {
        return miRolTactico switch
        {
            "Derecha" => jugador.position + (jugador.right * distanciaFlanqueo),
            "Izquierda" => jugador.position + (-jugador.right * distanciaFlanqueo),
            "Frente" => jugador.position + (jugador.forward * 1.5f),
            _ => jugador.position
        };
    }

    private void BuscarNuevoPuntoAleatorio()
    {
        Vector2 circuloAleatorio = Random.insideUnitCircle * radioPatrullaje;
        Vector3 direccionAleatoria = new Vector3(circuloAleatorio.x, 0, circuloAleatorio.y) + posicionInicial;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(direccionAleatoria, out hit, radioPatrullaje, NavMesh.AllAreas))
            agente.SetDestination(hit.position);
    }

    private void LiberarMiRol()
    {
        if (miRolTactico != "" && miRolTactico != "Espera" && CombatManager.Instancia != null)
        {
            CombatManager.Instancia.LiberarPosicion(miRolTactico);
            miRolTactico = "";
        }
    }

    private void OnDestroy()
    {
        // Si estaba en cola, salimos de ella antes de morir
        if (estadoActual == EstadoIA.Esperando && CombatManager.Instancia != null)
            CombatManager.Instancia.SalirDeCola(this);

        LiberarMiRol();
    }

    private void GritarOrdenTactica()
    {
        if (miRolTactico == "Izquierda")
        {
            string compañero = CombatManager.Instancia.ocupanteDerecha;
            Debug.Log(compañero != ""
                ? $"{miNombre}: ¡Te flanqueo por la izquierda, cúbreme {compañero}!"
                : $"{miNombre}: ¡Lo rodeo por la izquierda!");
        }
        else if (miRolTactico == "Derecha")
        {
            string compañero = CombatManager.Instancia.ocupanteFrente;
            Debug.Log(compañero != ""
                ? $"{miNombre}: ¡Voy por la derecha! ¡Mantenlo ocupado {compañero}!"
                : $"{miNombre}: ¡Avanzando por la derecha!");
        }
        else if (miRolTactico == "Frente")
        {
            Debug.Log($"{miNombre}: ¡Lo tengo en la mira! ¡Atacando!");
        }
    }

    // Gizmos: visión en amarillo, ataque en rojo, origen de la línea de visión en cian
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        // Marca la posición de los ojos
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + Vector3.up * alturaOjos, 0.1f);
    }

}

