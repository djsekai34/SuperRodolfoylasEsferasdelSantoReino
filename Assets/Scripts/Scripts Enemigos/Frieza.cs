using System.Collections;
using UnityEngine;

public class Frieza : MonoBehaviour
{
    [Header("Movimiento")]
    public Vector3 puntoDestinoXYZ;
    public float velocidad = 2f;
    private bool estaAtacando = false;
    private bool estaMuerto = false; 

    [Header("Configuración Raycast")]
    public Transform puntoDeEmision;
    public float distanciaRaycast = 5f;
    public LayerMask capaJugador;

    [Header("Ataque")]
    public GameObject Disco_Freezer;
    public float cadenciaAtaque = 0.8f;
    private float tiempoSiguienteAtaque = 0f;

    private Vector3 posicionInicial;
    private Vector3 objetivoActual;
    private Animator anim;

    void Start()
    {
        posicionInicial = transform.position;
        objetivoActual = puntoDestinoXYZ;

        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //Si hemos matado a Freezer bloqueamos todo el update
        if (estaMuerto) return;

        //Llamamos a la funcion Detectar ya para verificar si el raycast ha tocado al Rodolfo
        DetectarYAtacar();

        // Acualizamos el animator para ver si ataca o no
        if (anim != null)
        {
            anim.SetBool("Atacando", estaAtacando);
        }

        // Si no ataca Freezer volvera a su patrulla normal
        if (!estaAtacando)
        {
            MoverEnemigo();
        }
    }

    void DetectarYAtacar()
    {
        if (puntoDeEmision == null) return;

        //Calculamos de donde saldra el raycast que sera hacia la izquierda
        Vector3 direccionRayo = -transform.right;
        //Lanzamos el rayo pero ojo que solo busque a la capa jugador que hemos puesto lo demas da igual
        RaycastHit2D choque = Physics2D.Raycast(puntoDeEmision.position, direccionRayo, distanciaRaycast, capaJugador);

        //Hemos tocado algo y ese algop tiene el tag player
        if (choque.collider != null && choque.collider.CompareTag("Player"))
        {
            //Ponemos esto a true y hace la animacion de ataque
            estaAtacando = true;

            //Disparamos si el tiempo que lleva el nivel en marcha es mas alto que el tiempo del proximo disparo que hayamos puesto
            if (Time.time >= tiempoSiguienteAtaque)
            {
                //Lanzamos el disco y calculamos cuando sera el siguiente ataque
                LanzarDisco();
                tiempoSiguienteAtaque = Time.time + cadenciaAtaque;
            }
        }
        //Si el jugador sale del rango pues ya no ataca
        else
        {
            estaAtacando = false;
        }
    }

    void LanzarDisco()
    {
        if (Disco_Freezer != null)
        {
            // Creamos el disco de freezer desde el punto de emision que hayamos puesto
            Instantiate(Disco_Freezer, puntoDeEmision.position, transform.rotation);
        }
    }

    void MoverEnemigo()
    {

        //Movemos el enemigo con fluider hacia un punto con una velocidad constante
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);

        // ¿Ha llegado freezer a su destino?
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            //Si es que si lo giramos 180 grados y va hacia el punto de inicio
            if (objetivoActual == puntoDestinoXYZ)
            {
                objetivoActual = posicionInicial;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            // Ha llegado al inicio pues volvemos a ir al objetivo
            else
            {
                objetivoActual = puntoDestinoXYZ;
                transform.eulerAngles = new Vector3(0, 0, 0); 
            }
        }
    }

    public void Morir()
    {
        // Hacemos que freezr pueda morir solo 1 vez
        if (estaMuerto) return;

        // Marcamos los estados para detener cualquier logica que tengamos de freezer
        estaMuerto = true;
        estaAtacando = false;

        if (anim != null)
        {
            // Bajamos la velocidad de reproduccion de la animacion y la reproducimos
            anim.speed = 0.4f;
            anim.Play("MuerteFrieza");

            // Iniciamos la corutina
            StartCoroutine(EfectoEstatua());
        }
    }

    IEnumerator EfectoEstatua()
    {

        // Esperamos un frame para que el animator sepa que hemos pasado de andar o atacar a morir
        yield return null;

        // Calculamos el tiempo que dura la animacion de la muerte
        float duracionReal = anim.GetCurrentAnimatorStateInfo(0).length;

        // Calculamos la animacion con el tiempo que le hemos puesto
        float tiempoEspera = duracionReal / anim.speed;

        //Esperamos el 70% de la animacion
        yield return new WaitForSeconds(tiempoEspera * 0.70f);

        // Congelamos la animacion
        anim.speed = 0;

        //Desactivamos el collider para que lo atravesemos
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Desactivamos la simulación física por completo, lo frenamo, dejamos de empujarlo y dejamos de leer al objeto
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
            rb.simulated = false; 
        }

        // Hacemos a freezer transparente
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1, 1, 1, 0.6f);

    }
}