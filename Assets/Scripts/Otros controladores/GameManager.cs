using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int puntosTotales = 0;
    public static int vidasTotales = 3;
    public static string ultimoNivel;

    private static GameManager instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //Obtenemos el nombre de la escena actual donde este nuestro personaje
        string escena = SceneManager.GetActiveScene().name;

        // Guardamos todas las escena menos las que ponemos en el if
        if (escena != "Muerte" && escena != "Menu Principal" && escena != "MuerteSN" && escena != "SelectorNivel")
        {
            //Guardamos el nivel actual en una variable estatica
            ultimoNivel = escena;
        }
    }

    public static void RestarVida()
    {
        //Le quitamos una vida al contador
        vidasTotales--;
        
        //Entramos al if si hemos pedido la escena
        if (vidasTotales <= 0)
        {
            //Congelamos el juego y guardamos el nombre de la escena donde estamos actualmente
            Time.timeScale = 0f;
            string escenaActual = SceneManager.GetActiveScene().name;

            // Dependiendo de que nivel estemos jugando mandamos al jugador a una pantalla de muerte u otra
            if (escenaActual == "JaenSN" || escenaActual == "CazorlaSN" || escenaActual == "MartosSN" || escenaActual == "AndujarSN" || escenaActual == "UbedaSN" || escenaActual == "TorredonjimenoSN" || escenaActual == "AlcalaLaRealSN")
            {
                SceneManager.LoadScene("MuerteSN");
            }
            else if (escenaActual == "Jaen" || escenaActual == "Cazorla" || escenaActual == "Martos" || escenaActual == "Andujar" || escenaActual == "Ubeda" || escenaActual == "Torredonjimeno" || escenaActual == "AlcalaLaReal")
            {
                SceneManager.LoadScene("Muerte");
            }
            //Ponemos velocidad de niuevo
            Time.timeScale = 1f;
        }
    }

    //Cuando cogemos 1 bola sumamos un punto
    public static void SumarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
    }
    
    //Reseteamos los datos del juego
    public static void ResetearDatos()
    {
        puntosTotales = 0;
        vidasTotales = 3;
    }

    public static void SumarVida()
    {
        // Sumamos una vida
        vidasTotales++;
    }
}