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
        // Buscamos en el json los datos
        DatosJuego datos = SaveGame.Cargar();

        // Si nos hemos pasado el modo historia, al selector con el nivel extra desbloqueado, si no al selector sin el nivel extra
        if (datos.modoHistoriaCompletado == 1)
        {
            SceneManager.LoadScene("SelectorNivelPass");
        }
        else
        {
            SceneManager.LoadScene("SelectorNivel");
        }
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
        // Leemos el estado de nuestro archivo en el disco duro
        DatosJuego datosActuales = SaveGame.Cargar();

        // Modificamos el dato que nos intersesa
        datosActuales.modoHistoriaCompletado = 1;

        // Sobreescribimos el json de nuevo
        SaveGame.Guardar(datosActuales);

        // Cargamos la escena
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
        // Leemos el estado de nuestro archivo en el disco duro
        DatosJuego datosActuales = SaveGame.Cargar();

        // Modificamos el dato que nos intersesa
        datosActuales.saltarCinematicaActivo = 1;

        // Sobreescribimos el json de nuevo
        SaveGame.Guardar(datosActuales);

        // Cargamos la escena
        SceneManager.LoadScene(nombreCinematica);
    }
}