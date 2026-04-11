using UnityEngine;

public class BolasMagicas : MonoBehaviour
{
    int valorPuntos = 1;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Detectamos si Rodolfo ha tocado la bola
        if (otro.CompareTag("Player"))
        {
            RecogerBola();
        }
    }

    private void RecogerBola()
    {
        // Sumamos los puntos al contador global del GameManager
        GameManager.puntosTotales += valorPuntos;

        // Destruimos la bola inmediatamente
        Destroy(gameObject);
    }
}