using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDeNivel : MonoBehaviour
{
    [Header("Ajustes del Nivel")]
    [SerializeField] private string nombreSiguienteNivel = "Martos";
    [SerializeField] private float tiempoParaCargar = 5f;
    [SerializeField] private int bolasNecesarias = 2; // Variable para no repetir código manualmente

    public static bool rodolfoEsInmortal = false;

    private float cronometro = 0f;
    private bool rodolfoEstaEncima = false;
    private bool cargando = false;

    void Update()
    {
        // Si Rodolfo esta encima y tiene las bolas magicas necesarias y no ha empezado a cargar el nivel
        if (rodolfoEstaEncima && GameManager.puntosTotales >= bolasNecesarias && !cargando)
        {
            // Cada actualizacion de frame vamos sumando tiempo
            cronometro += Time.deltaTime;

            //Si el cronometro llega al tiempo defenido anteriormente carga la siguiente escena
            if (cronometro >= tiempoParaCargar)
            {
                CargarSiguienteEscena();
            }
        }
        //Si nos salimos se resetea el tiempo
        else if (!rodolfoEstaEncima)
        {
            cronometro = 0f;
        }
    }

    //Verificamos si ha entrado rodolfo si esta dentro activiamos los 5s y lo hacemos inmortal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = true;
            rodolfoEsInmortal = true;
        }
    }

    //Aqui hacemos lo contrario le quitamos la inmortalidad y resteamos la cuenta
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = false;
            rodolfoEsInmortal = false;
            cargando = false;
        }
    }

    void CargarSiguienteEscena()
    {
        cargando = true; //Condicion de seguridad para que solo se ejecute otra vez
        rodolfoEsInmortal = false; // Le quitamos la inmortalidad
        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}