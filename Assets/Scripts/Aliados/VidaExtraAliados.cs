using UnityEngine;

public class VidaExtraAliados : MonoBehaviour
{
    [Header("Ajustes de Tags")]
    public string tagJugador = "Player";
    public string tagAliados = "GokuVida";

    private AudioSource miAltavoz;
    private SpriteRenderer miSprite;
    private Collider2D miCollider;

    private void Awake()
    {
        // Ahora que todo está en el mismo sitio, lo pillamos directamente
        miAltavoz = GetComponent<AudioSource>();
        // Si el SpriteRenderer sigue en el hijo, usamos Children, si lo subiste, quita el Children
        miSprite = GetComponentInChildren<SpriteRenderer>();
        miCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag(tagAliados) || otro.CompareTag(tagJugador))
        {
            // 1. Lógica de juego
            GameManager.SumarVida();

            // 2. Lógica de audio
            if (miAltavoz != null && miAltavoz.clip != null)
            {
                miAltavoz.Play();

                // 3. ¡DESAPARECER!
                // Apagamos el visual (hijo o padre) y el choque (padre)
                if (miSprite != null) miSprite.enabled = false;
                if (miCollider != null) miCollider.enabled = false;

                // Destruimos el objeto entero cuando acabe el sonido
                Destroy(gameObject, miAltavoz.clip.length);
            }
            else
            {
                // Si no hay audio, se va al instante
                Destroy(gameObject);
            }
        }
    }
}