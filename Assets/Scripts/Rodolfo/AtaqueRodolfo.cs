using UnityEngine;

public class AtaqueRodolfo : MonoBehaviour
{
    void Start()
    {
        // Se destruye automáticamente a los 3 segundos si no choca con nada
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Entramos al if si hemos tocado algo que tenga el tag enemy
        if (collision.CompareTag("Enemy"))
        {
            // 1. ¿Es Broly?
            Broly scriptBroly = collision.GetComponent<Broly>();
            if (scriptBroly != null)
            {
                scriptBroly.Morir();
            }

            // 2. ¿Es Freezer?
            Frieza scriptFreezer = collision.GetComponent<Frieza>();
            if (scriptFreezer != null)
            {
                scriptFreezer.Morir();
            }

            // 3.  ¿Es Kefla?
            Kefla scriptKefla = collision.GetComponent<Kefla>();
            if (scriptKefla != null)
            {
                scriptKefla.Morir();
            }

            // 4.  ¿Es Janemba?
            Janemba scriptJanemba = collision.GetComponent<Janemba>();
            if (scriptJanemba != null)
            {
                scriptJanemba.Morir();
            }

            // 5. ¿Es Hit?
            Hit scriptHit = collision.GetComponent<Hit>();
            if (scriptHit != null)
            {
                scriptHit.Morir();
            }

            // 6. ¿Es Turles?
            Turles scriptTurles = collision.GetComponent<Turles>();
            if (scriptTurles != null)
            {
                scriptTurles.Morir();
            }

            // 7. ¿Es Cell?
            Cell scriptCell = collision.GetComponent<Cell>();
            if (scriptCell != null)
            {
                scriptCell.Morir();
            }

            // La bola de Rodolfo desaparece tras el impacto
            Destroy(gameObject);
        }
    }
}