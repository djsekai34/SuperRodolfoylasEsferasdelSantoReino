using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GanarNivelExtra : MonoBehaviour
{
    [Header("Configuración de Victoria")]
    public string nombreEscenaVictoria;
    public float tiempoEspera = 2.0f;

    private bool cambiandoEscena = false;
    private bool puedeComprobar = false; 

    void Start()
    {
        // Al empezar esperamos un poco para que unity compruebe todo
        StartCoroutine(EsperarInicioDeNivel());
    }

    IEnumerator EsperarInicioDeNivel()
    {
        // Esperamos 1.5 segundos para que a Unity le dé tiempo a cargar los enemigos
        yield return new WaitForSeconds(1.5f);
        puedeComprobar = true;
    }

    void Update()
    {
        // Si no ha pasado el tiempo de segurtidad que hemos puesto nos salimos
        if (!puedeComprobar) return;

        // Si no hemos cambiado de escena y el contado es 0 pues emepzamos la corrutina para empezar la cinematica
        if (!cambiandoEscena && GameManager.enemigosRestantes <= 0)
        {
            StartCoroutine(PasarDeNivel());
        }
    }

    // corrutina para cambiar la escena
    IEnumerator PasarDeNivel()
    {
        cambiandoEscena = true;
        yield return new WaitForSeconds(tiempoEspera);

        if (!string.IsNullOrEmpty(nombreEscenaVictoria))
        {
            SceneManager.LoadScene(nombreEscenaVictoria);
        }
    }
}
