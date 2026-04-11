using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDeNivelSN : MonoBehaviour
{
    [Header("Ajustes del Nivel")]
    public string nombreSiguienteNivel = "SelectorNivel";
    public float tiempoParaCargar = 5f;

    // Estático para que el script de salud de Rodolfo pueda leerlo fácilmente
    public static bool rodolfoEsInmortal = false;

    private float cronometro = 0f;
    private bool rodolfoEstaEncima = false;
    private bool cargando = false;

    void Update()
    {
        if (rodolfoEstaEncima && GameManager.puntosTotales >= 1 && !cargando)
        {
            cronometro += Time.deltaTime;

            if (cronometro >= tiempoParaCargar)
            {
                CargarSiguienteEscena();
            }
        }
        else
        {
            cronometro = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = true;
            rodolfoEsInmortal = true;
        }
    }

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
        cargando = true;
        rodolfoEsInmortal = false; 
        SceneManager.LoadScene(nombreSiguienteNivel);
    }
}