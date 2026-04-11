using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControladorSelectorNivel : MonoBehaviour, IPointerClickHandler
{
    public string nombreDeEscena;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Si hemos puesto bien el nivel de Selector del nivel y le damos clic cargamos la escena reseteando los datos 
        if (!string.IsNullOrEmpty(nombreDeEscena))
        {
            GameManager.ResetearDatos();
            SceneManager.LoadScene(nombreDeEscena);
        }
    }

    // Escenas de creditos
    public void OnBotonCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    // Volver al menu
    public void OnBotonVolverAlMenu()
    {
        // Nos aseguramos de que el tiempo esté a 1 por si venimos de una pausa
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Principal");
    }
}