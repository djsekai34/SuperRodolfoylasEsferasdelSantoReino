using System.Collections;
using UnityEngine;

public class Janemba : MonoBehaviour
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
    public GameObject Disco_Janemba;
    public float cadenciaAtaque = 0.5f; 

    private Vector3 posicionInicial;
    private Vector3 objetivoActual;
    private Animator anim;
    private Coroutine corrutinaAtaque;

    void Start()
    {
        // Guardamos la posición donde aparece el enemigo y establecemos que el primer lugar a donde quiere ir el enemigo
        posicionInicial = transform.position;
        objetivoActual = puntoDestinoXYZ;
        //Buscamos el componeter animator en su hijo
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si el enemigo esta muerto nos salimos sin hacer nada
        if (estaMuerto) return;

        // Ejecutamos la logica para detectar si hay alguien al quien atacar
        DetectarYAtacar();

        // Si existe el animator le pasamos lo que contenta estaAtacando para saber si atacar o andar
        if (anim != null)
        {
            anim.SetBool("Atacando", estaAtacando);
        }

        // Si no esta atacando significa que nos estamos moviendo
        if (!estaAtacando)
        {
            // Ponemos la velovidad normal y movemos al enemigo
            if (anim != null) anim.speed = 1f;
            MoverEnemigo();
        }
    }

    void DetectarYAtacar()
    {
        // Comprobacion de que hemos asigando el punto de salida si no nos salimos para no tener errores
        if (puntoDeEmision == null) return;

        // Definimos a donde va el rayo del enemig y lanzamos un raycast con una distancia maxima y buscando al jugador
        Vector3 direccionRayo = -transform.right;
        RaycastHit2D choque = Physics2D.Raycast(puntoDeEmision.position, direccionRayo, distanciaRaycast, capaJugador);

        // Si el rayo choca con algo y tiene el tag player
        if (choque.collider != null && choque.collider.CompareTag("Player"))
        {
            // Si no esta atacando entramos a atacar y lanzamos la rafaga de bolas
            if (!estaAtacando)
            {
                estaAtacando = true;
                corrutinaAtaque = StartCoroutine(RafagaDeDiscos());
            }
        }
        // Si rodolfo se ha escapado o el rayo no toca nada 
        else
        {
            // Si estabamos atacando paramos de atacar y paramos la corrutina de lanzar bolas
            if (estaAtacando)
            {
                estaAtacando = false;
                if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);
            }
        }
    }

    IEnumerator RafagaDeDiscos()
    {
        if (anim != null)
        {
            // Ponemos la animacion a velcoidad normal, le asiganmos el nombre de la animacion y forzamos que empiece desde principio
            anim.speed = 1f;
            string nombreAnim = "JanembaAtaque";
            anim.Play(nombreAnim, 0, 0f);

            // Esperamos un poquito para que unity procese el cambio
            yield return new WaitForSeconds(0.1f);

            // Buscamos cuanto dura la animacion en segundos si por alguna casualidad da error  le ponemos 1s por defecto
            float duracion = anim.GetCurrentAnimatorStateInfo(0).length;
            if (duracion <= 0) duracion = 1f;

            // Espertamos a que la animcion llegue casi al final
            yield return new WaitForSeconds(duracion * 0.85f);

            // Lo dejamos clavado en el ultimo frame, forzamos que se actualice paramos la velocidad para que se quede congelado
            anim.Play(nombreAnim, 0, 0.99f);
            anim.Update(0);
            anim.speed = 0f;
        }

        // Lanzamos el primer ataque si o si
        LanzarDisco();

        // Si rodolfo esta vivo y jamenba tambien
        while (estaAtacando && !estaMuerto)
        {
            // Esperamos el tiempo de cadencia entre disparos
            yield return new WaitForSeconds(cadenciaAtaque);
            // Verificamos de nuevo por si acaso ha ocurrido algo y lanzamos otra bola
            if (estaAtacando && !estaMuerto)
            {
                LanzarDisco();
            }
        }
    }

    // Lanzamos la bola
    void LanzarDisco()
    {
        if (Disco_Janemba != null)
            Instantiate(Disco_Janemba, puntoDeEmision.position, transform.rotation);
    }

    // Movimiento del enemigo
    void MoverEnemigo()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            if (objetivoActual == puntoDestinoXYZ)
            {
                objetivoActual = posicionInicial;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                objetivoActual = puntoDestinoXYZ;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    public void Morir()
    {
        // Si el enemigo esta muerto nos salimos y activamos que es un cadaver y deja de ser una amenaza
        if (estaMuerto) return;
        estaMuerto = true;
        estaAtacando = false;

        // Si estaba lanzando bolas lo paramos inmediatamente 
        if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);

        // Desactivamos su rigibody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        // Todavia no ponemos trigger al collider dle enemigo lo gestionara la corrutina
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = false;

        // Congelamos la animacion normal y empezamos la corrutina
        if (anim != null)
        {
            anim.speed = 0; 
            StartCoroutine(EfectoEstatua());
        }
    }

    IEnumerator EfectoEstatua()
    {
        // Localizamos el sprite renderer para moverlo y cambiar el color, si por algun caso no hay lo paramos para que no de error
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;

        float distanciaAlSueloFinal = 1f;
        // Lanzamos un raycast hacia abajo para saber a que distancia esta del suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 15f, LayerMask.GetMask("Ground"));
        if (hit.collider != null) distanciaAlSueloFinal = hit.distance;

        // Guardamos la posicion del sprite actual y donde debe de acabar
        Vector3 posInicialHijo = sr.transform.localPosition;
        Vector3 posFinalHijo = posInicialHijo - new Vector3(0, distanciaAlSueloFinal, 0);


        // El frame 3 lo levamtamos un poco para que no atraviese el suelo
        float levantamientoF3 = 0.6f;
        Vector3 posFrame3Hijo = posFinalHijo + new Vector3(0, levantamientoF3, 0);

        if (anim != null) anim.speed = 0; // Paramos todas las animacion

        float tiempo = 0;
        float duracionCaida = 1.5f;

        while (tiempo < duracionCaida)
        {
            // Suamamos el tiempo que pasa entre frames y su progreso que es de 0 a 1
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionCaida;

            // Emepezamos la caida visual que es del frame 2
            if (progreso >= 0.33f)
            {
                // Normalizamos el progreso
                float pCaida = (progreso - 0.33f) / 0.67f;
                // Usamos SmoothStep para que la animacion sea fluida y no seca alc aer
                float pSuave = Mathf.SmoothStep(0, 1, pCaida);
                // Movemos el sprite del sitio original al punto del frame 3 que hemos definido
                sr.transform.localPosition = Vector3.Lerp(posInicialHijo, posFrame3Hijo, pSuave);

                // Bajamos la opacidad el enemigo
                sr.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.9f), pCaida);
            }

            // Sincronizamos la animacion para recorra desde el primer frame hasta el 3
            anim.Play("JanembaMuere", 0, progreso * 0.66f);
            yield return null;
        }

        // Formzamos que el sprite se quede en la poscion del frame 3, le bajamos las transparencia y congelamos la animacion
        sr.transform.localPosition = posFrame3Hijo;
        sr.color = new Color(1, 1, 1, 0.8f);
        anim.Play("JanembaMuere", 0, 0.66f);

        // Cambiamos su collider a trigger para que sea atravesado
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Pausamos de forma epica la animacion para que el jugador vea que lo ha derrotado
        yield return new WaitForSeconds(0.4f);

        // Saltamos de golpe a la poscion de la variable posFinalHijo sin quqe se note
        sr.transform.localPosition = posFinalHijo;

        // Dejamos la transparencia final
        sr.color = new Color(1, 1, 1, 0.6f);

        // Saltamos al ultimo frame
        anim.Play("JanembaMuere", 0, 1.0f);

        // Un pequeño respiro para que se asiente la pose en nuestras pantallas
        yield return new WaitForSeconds(0.2f);

        // Paramos todo  y aseguramos la poscion por ultima vez
        anim.speed = 0;
        sr.transform.localPosition = posFinalHijo;

        // Cambiamos el tag del enemigo a EnemigoMuerto para el tema del ultimo nivel
        gameObject.tag = "EnemigoMuerto";
    }
}