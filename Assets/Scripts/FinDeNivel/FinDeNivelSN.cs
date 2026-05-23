using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDeNivelSN : MonoBehaviour
{
    [Header("Ajustes del Nivel")]
    public string nombreSiguienteNivel = "SelectorNivel";
    public float tiempoParaCargar = 5f;

    public static bool rodolfoEsInmortal = false;

    private float cronometro = 0f;
    private bool rodolfoEstaEncima = false;
    private bool cargando = false;

    void Update()
    {
        // Si Rodolfo esta encima y ha obtenido la bola magica y no se ha cargado la siguiente escena
        if (rodolfoEstaEncima && GameManager.puntosTotales >= 1 && !cargando)
        {
            // Vamos sumando el tiempo real
            cronometro += Time.deltaTime;

            // Si el tiempo real ha superado o igualado al tiempo que hemos establecido nos vamos a la siguiente escena
            if (cronometro >= tiempoParaCargar)
            {
                CargarSiguienteEscena();
            }
        }
        // En el caso que nos salgamos resteamos el contador
        else
        {
            cronometro = 0f;
        }
    }

    // Si Rodolfo esta dentro lo hacemos inmortal y informamos que esta dentro
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = true;
            rodolfoEsInmortal = true;
        }
    }
    // Si Rodolfo esta fuera pues ya no es inmortal informamos que no esta dentro y no cargamos nada
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = false;
            rodolfoEsInmortal = false;
            cargando = false;
        }
    }

    // Cambiamos la variable de cargar la escena a true, permitimos el poder cargar le quitamos la inmortalidad y no la llevamos a la siguiente escena
    void CargarSiguienteEscena()
    {
        cargando = true;
        rodolfoEsInmortal = false; 
        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}