using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int puntosTotales = 0;
    public static int vidasTotales = 3;
    public static string ultimoNivel;


    private static int puntosSuelo = 0;
    private static int vidasSuelo = 3;

    public static int enemigosRestantes = 0;

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

    // Nos suscribimos al evento que carga la escena para qie acrive el script
    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    // Nos desuscribimos al destruirse para evitar problemas
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    // Este metodo se va a ejcutar cada vez que cambiemos de escena
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        // Si llegamos al nivel extra, le damos una vida al jugador por el regalito y actualizamos el suelo de las vidas para que, si reinicia el nivel desde la pausa, conserve este regalo.
        if (escena.name == "Nivel Extra" || escena.name == "Nivel ExtraSN")
        {
            SumarVida();
            // Si no actualizamos el suelo, al reiniciar el nivel desde la pausa, el jugador perdería esta vida extra que ha ganado al entrar al nivel extra, lo cual no sería justo.
            vidasSuelo = vidasTotales;

        }
    }

    // Este metodo lo llama el script FinDeNivel cuando el jugador vaya a pasar al siguiente nivel
    public static void GuardarProgresoNivel()
    {
        // Guardamos la vidas actuales y las bolas cogidas actualmente
        puntosSuelo = puntosTotales;
        vidasSuelo = vidasTotales; // Guardamos las vidas que le quedan al pasar de nivel
    }

    // Este metodo lo llamara el boton reiniciar cuando le demos en el menu de pausa
    public static void ResetearPuntosParaReiniciar()
    {
        // Si reiniciamos el nivel recuperamos la vidas y bolas cogidas que teniamos al entrar al nivel, dando igual lo que hayamos cogido en este momento
        puntosTotales = puntosSuelo;
        vidasTotales = vidasSuelo; 
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

        // Si estamos en el nivel extra, contamos cu�ntos objetos con el tag "Enemy" quedan vivos
        if (escena == "Nivel Extra" || escena == "Nivel ExtraSN")
        {
            enemigosRestantes = GameObject.FindGameObjectsWithTag("Enemy").Length;
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
            if (escenaActual == "JaenSN" || escenaActual == "CazorlaSN" || escenaActual == "MartosSN" || escenaActual == "AndujarSN" || escenaActual == "UbedaSN" || escenaActual == "TorredonjimenoSN" || escenaActual == "AlcalaLaRealSN" || escenaActual == "Nivel ExtraSN")
            {
                SceneManager.LoadScene("MuerteSN");
            }
            else if (escenaActual == "Jaen" || escenaActual == "Cazorla" || escenaActual == "Martos" || escenaActual == "Andujar" || escenaActual == "Ubeda" || escenaActual == "Torredonjimeno" || escenaActual == "AlcalaLaReal" || escenaActual == "Nivel Extra")
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
        puntosTotales = 0; // Resetamos la vida que lleva encima en ese momento
        vidasTotales = 3; // Le damos la tope de Vida a Rodolfo que son 3
        puntosSuelo = 0; // Limpiamos las bolas cogidas, lo ponemos a 0 porque si viene de una partida anterior no se acumulen
        vidasSuelo = 3; // Limpiamos la vida de Rodolfo, para que siempre sea su salud incial 3 ya sea que venimos de una partida anterior o reiniciamos en el nivel 1
    }

    public static void SumarVida()
    {
        // Sumamos una vida
        vidasTotales++;
    }
}