using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class HudController : MonoBehaviour
{
    public TextMeshProUGUI textoBolasMagicas;
    public TextMeshProUGUI textoVidas;
    public Image imagenBolas;
    public Image imagenEnemigos;

    void Update()
    {
        // Actualziamos la vida del jugador en cada frame con los datos que nos llegue desde el gamemanager
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + GameManager.vidasTotales;

        // Si estamos en el nivel extra en vez de mostrar las bolas mostramos los enemigos
        if (SceneManager.GetActiveScene().name == "Nivel Extra" || SceneManager.GetActiveScene().name == "Nivel ExtraSN")
        {
            // Si existe el textoBolasMagicas cambiamos su texto a enemigos y los enemigos restates que quede que nos dara el gamemanager
            if (textoBolasMagicas != null)
            {
                textoBolasMagicas.text = "Enemigos: " + GameManager.enemigosRestantes;
                textoBolasMagicas.fontSize = 34;
            }

            // Ocultamos la imagen de las bolas y mostramos la del enemigo
            if (imagenBolas != null) imagenBolas.gameObject.SetActive(false);
            if (imagenEnemigos != null) imagenEnemigos.gameObject.SetActive(true);
        }
        // Si no estamos en el nivel extra mostramos las bolas magicas
        else
        {
            if (textoBolasMagicas != null)
            {
                textoBolasMagicas.text = "Bolas: " + GameManager.puntosTotales;
                textoBolasMagicas.fontSize = 36; 
            }

            if (imagenBolas != null) imagenBolas.gameObject.SetActive(true);
            if (imagenEnemigos != null) imagenEnemigos.gameObject.SetActive(false);
        }
    }
}