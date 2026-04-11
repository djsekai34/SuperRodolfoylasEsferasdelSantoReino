using UnityEngine;
using TMPro;
public class HudController : MonoBehaviour
{
    public TextMeshProUGUI textoBolasMagicas;
    public TextMeshProUGUI textoVidas;

    void Update()
    {
        // Actualizamos los textos cada frame con los datos del GameManager
        if (textoBolasMagicas != null)
            textoBolasMagicas.text = "Bolas: " + GameManager.puntosTotales;

        if (textoVidas != null)
            textoVidas.text = "Vidas: " + GameManager.vidasTotales;
    }
}
