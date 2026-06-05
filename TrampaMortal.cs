using UnityEngine;
using UnityEngine.SceneManagement;

public class TrampaMortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Recarga la escena actual. ¡Recuerda agregar la escena en Build Settings!
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
