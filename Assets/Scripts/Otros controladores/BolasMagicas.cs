using UnityEngine;

public class BolasMagicas : MonoBehaviour
{
    int valorPuntos = 1;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Detectamos si Rodolfo ha tocado la bola
        if (otro.CompareTag("Player"))
        {
            // Cogemos el componente de Rodolfo
            Rodolfo rodolfo = otro.GetComponent<Rodolfo>();

            if (rodolfo != null)
            {
                // Cogemos el sprite de esta bola del escenario
                SpriteRenderer miSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

                if (miSpriteRenderer != null)
                {
                    // Le mandamos al metodo de rodolfo la bola exacta
                    rodolfo.ActualizarBolaCabeza(miSpriteRenderer.sprite);
                }
            }

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