using UnityEngine;

public class VidaExtraAliados : MonoBehaviour
{
    [Header("Ajustes de Tags")]
    public string tagJugador = "Player";

    private AudioSource miAltavoz;
    private SpriteRenderer miSprite;
    private Collider2D miCollider;

    private void Awake()
    {
        // Cogemso el componete del audio, el sprite y el collider
        miAltavoz = GetComponent<AudioSource>();
        miSprite = GetComponentInChildren<SpriteRenderer>();
        miCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Si lo que le ha tocado ha sido algo que contenga el tag asigando
        if (otro.CompareTag(tagJugador))
        {
            // Llamamos al gamenanager para que nos sube la vida
            GameManager.SumarVida();

            // Si tenemos el altavoz asigando y el clip asignado
            if (miAltavoz != null && miAltavoz.clip != null)
            {
                // Reproducimos el audio asignado y apagamos visualemtne tanto el sprite como el collider
                miAltavoz.Play();
                if (miSprite != null) miSprite.enabled = false;
                if (miCollider != null) miCollider.enabled = false;

                // Destruimos el objeto cuando se acabe el audio de reproducirse
                Destroy(gameObject, miAltavoz.clip.length);
            }
            else
            {
                // Si por alguan casualidad no hay audio o altavoz solo lo destruimos y ya
                Destroy(gameObject);
            }
        }
    }
}