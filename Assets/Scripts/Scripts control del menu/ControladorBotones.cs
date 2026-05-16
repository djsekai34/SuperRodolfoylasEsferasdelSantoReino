using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorBotones : MonoBehaviour
{
    public void OnBotonHistoria()
    {
        GameManager.ResetearDatos();
        SceneManager.LoadScene("Jaen");
    }

    public void OnBotonCinematicaFinal()
    {
        SceneManager.LoadScene("CinematicaFinal");
    }

    public void OnBotonCinematicaFinalMalo()
    {
        SceneManager.LoadScene("CinematicaFinalMalo");
    }

    public void OnBotonNivelExtra()
    {
        SceneManager.LoadScene("Nivel Extra");
    }

    public void OnBotonGracias()
    {
        SceneManager.LoadScene("Gracias");
    }

    public void OnBotonCinematicaInicial()
    {
        SceneManager.LoadScene("CinematicaInicial");
    }

    public void OnBotonSelectorNiveles()
    {
        SceneManager.LoadScene("SelectorNivel");
    }

    public void OnBotonCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void OnBotonSalir()
    {
        Application.Quit();
    }

    public void OnBotonControles()
    {
        SceneManager.LoadScene("Controles");
    }

    public void OnBotonVolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Principal");
    }

    public void OnBotonVolverAlMenuGracias()
    {
        // Guardamos en el disco duro que el jugador ya se ha pasado el juego
        PlayerPrefs.SetInt("ModoHistoriaCompletado", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu Principal");
    }

    public void OnBotonReiniciarNivel()
    {
        //Descongelamos el nivel y miramos qque pantalla de muerte estamos actualmente 
        Time.timeScale = 1f;
        string escenaDeMuerte = SceneManager.GetActiveScene().name;

        // Decidimos qué hacer con los datos según la pantalla de muerte que usemos
        if (escenaDeMuerte == "MuerteSN")
        {
            // Si entramos aqui es que hemos muerto en un nivel de Selector de Nivel lo reseteamos todo al 0
            GameManager.ResetearDatos();
        }
        else
        {
            // Si es nivel de modo historia lo reiniciamos con las vidas que tenia
            GameManager.ResetearPuntosParaReiniciar();
        }

        // Si tenemos un nivel guardado en el game manager lo cargamos si no al menu principal
        if (!string.IsNullOrEmpty(GameManager.ultimoNivel))
        {
            SceneManager.LoadScene(GameManager.ultimoNivel);
        }
        else
        {
            SceneManager.LoadScene("Menu Principal");
        }
    }

    public void OnBotonSaltarYMostrarCinematica(string nombreCinematica)
    {
        // Guardamos en el disco de inmediato que queremos saltar la secuencia
        PlayerPrefs.SetInt("SaltarCinematicaActivo", 1);
        PlayerPrefs.Save();

        // Cargamos la escena de la cinemática que le pasemos por el parámetro
        SceneManager.LoadScene(nombreCinematica);
    }
}