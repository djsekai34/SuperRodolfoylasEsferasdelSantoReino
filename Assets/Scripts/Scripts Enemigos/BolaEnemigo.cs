using UnityEngine;

public class BolaEnemigo : MonoBehaviour
{
    public float velocidadDisco = 10f;

    [Header("Ajustes de Destrucción")]
    public float tiempoDeVida = 3f; // Segundos que tarda en borrarse sola

    void Start()
    {
        // Nada más nacer, le decimos a Unity que se cargue este objeto en X segundos
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // El ataque se mueve a la izquierda respecto a su propia rotación
        transform.Translate(Vector3.left * velocidadDisco * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            // Buscamos el script Rodolfo en el objeto chocado
            Rodolfo rodolfo = otro.GetComponent<Rodolfo>();

            if (rodolfo != null)
            {
                // Le damos el golpe
                rodolfo.TomarDano();
            }

            // El disco desaparece al chocar
            Destroy(gameObject);
        }
    }
}