using System.Collections;
using UnityEngine;

public class HitMenu : MonoBehaviour
{
    private Animator anim;
    private float escalaOriginalX; 

    [Header("Configuración Animación")]
    public string nombreAnimacion = "HitMP";

    [Header("Puntos de Movimiento")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidadMovimiento = 50f;

    
    private bool haciaB = false;

    void Start()
    {
        // Cogemos el animator
        anim = GetComponent<Animator>();

        // Guardamos la escala original de los sprite
        escalaOriginalX = transform.localScale.x;

        // Si existe el animator le damos play a la animacion de Hit para que empiece el bucle
        if (anim != null)
        {
            anim.Play(nombreAnimacion);
        }

        // Si nos falta algun punto donde ira Hit lo aviamos mediante un warning
        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning("Nos falta algun punto no podemos hacer la animacion");
        }
    }

    void Update()
    {
        // Si los puntos existen empezamos el movimiento
        if (puntoA != null && puntoB != null)
        {
            // Creamos una varaible para guardar el destino del personaje
            Vector3 destino;

            // Elegimos el destino si es el punto A o B
            if (haciaB)
            {
                destino = puntoB.position;
            }
            else
            {
                destino = puntoA.position;
            }

            // Movemos el personaje hacia el destino elegido
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);

            // Si vamos al punto B que sera la derecha lo giramos respetando la escala original
            if (haciaB)
            {
                transform.localScale = new Vector3(-escalaOriginalX, transform.localScale.y, transform.localScale.z);
            }
            // Si vamos al punto A que sera la izquierda vamos respetando la escala original
            else
            {
                transform.localScale = new Vector3(escalaOriginalX, transform.localScale.y, transform.localScale.z);
            }

            // Si la distancia es muy pequeña es que ya hemos llegado al punto
            if (Vector3.Distance(transform.position, destino) < 0.1f)
            {
                // Cambiamos el destino del personaje
                haciaB = !haciaB;
            }
        }
    }
}