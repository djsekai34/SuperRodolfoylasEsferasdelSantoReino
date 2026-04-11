using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorBotones : MonoBehaviour
{
    public void OnBotonHistoria()
    {
        GameManager.ResetearDatos();
        SceneManager.LoadScene("Jaen");
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
            // Si es nivel de modo historia solo le devolvemos las vidas
            GameManager.vidasTotales = 3;
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
}