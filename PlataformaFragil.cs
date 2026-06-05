using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlataformaFragil : MonoBehaviour
{
    [Header("Tiempos de Caída")]
    [SerializeField] private float tiempoParaCaer = 0.5f;
    [SerializeField] private float tiempoParaDestruir = 2f;

    private Rigidbody rb;
    private Collider colisionadorSolido;
    private bool yaPisada = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Clean Code: Escaneamos los colliders de este cubo para encontrar cuál es el sólido
        Collider[] misColliders = GetComponents<Collider>();
        foreach (Collider col in misColliders)
        {
            if (!col.isTrigger)
            {
                // Lo guardamos en memoria para usarlo después
                colisionadorSolido = col;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Revisamos si el que chocó (o su padre) es el Player
        bool esElJugador = other.CompareTag("Player") || other.transform.root.CompareTag("Player");

        if (esElJugador && !yaPisada)
        {
            yaPisada = true;
            StartCoroutine(SecuenciaDeCaida());
        }
    }

    private IEnumerator SecuenciaDeCaida()
    {
        Debug.Log("1. Corrutina iniciada. Esperando " + tiempoParaCaer + " segundos...");
        yield return new WaitForSeconds(tiempoParaCaer);

        Debug.Log("2. Apagando collider sólido...");
        if (colisionadorSolido != null)
        {
            colisionadorSolido.enabled = false;
        }

        Debug.Log("3. Soltando físicas...");
        rb.WakeUp(); // Unity 6: Forzamos al motor a actualizar este Rigidbody
        rb.isKinematic = false;
        rb.useGravity = true;

        // Le damos un empujón violento hacia abajo. 
        // Si con esto cae, significa que la gravedad global de tu proyecto está en 0.
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);

        yield return new WaitForSeconds(tiempoParaDestruir);

        Debug.Log("4. Destruyendo plataforma.");
        gameObject.SetActive(false);
    }
}
