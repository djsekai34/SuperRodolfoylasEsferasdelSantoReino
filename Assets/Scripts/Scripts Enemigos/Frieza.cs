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
        // Verificación de componentes por seguridad para que no falte ninguno
        Animator anim = GetComponentInChildren<Animator>();
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Si no tenemos el animador o el sprite lo cancelamos para evitar errores
        if (anim == null || sr == null) yield break;

        // Por defecto decimos que esta a un metro pero lazamos un raycast hacia abajo
        float distanciaAlSueloFinal = 1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 15f, LayerMask.GetMask("Ground"));
        // Si el rayo que lanzamos toca el suelo lo guardamos para que el cuerpo caiga perfecto
        if (hit.collider != null) distanciaAlSueloFinal = hit.distance;

        // Guardamos la posicion del sprite antes de morir
        Vector3 posOriginalHijo = sr.transform.localPosition;

        // En margen F3 es para cuando se quede de rodillas para que este un poco mas alto
        float margenF3 = 0.68f;
        Vector3 posSueloF3 = posOriginalHijo - new Vector3(0, distanciaAlSueloFinal - margenF3, 0);

        // El margen F4 es para cuando ya este en el suelo
        float margenF4 = 0.2f;
        Vector3 posFinalSueloF4 = posOriginalHijo - new Vector3(0, distanciaAlSueloFinal - margenF4, 0);

        // Frenamos todo para controlarlo nosotros
        anim.speed = 0;
        string nombreAnimMuere = "MuerteFrieza";
        float tiempo = 0;
        float duracionTotal = 2.0f;

        while (tiempo < duracionTotal)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionTotal;

            // Lo primeros segundos se queda donde estaba
            if (progreso < 0.70f)
            {
                sr.transform.localPosition = posOriginalHijo;
            }
            else
            {
                // En el ultimo 30% del tiempo restante d ela animacion calculamos la bajada del sprite 
                float pBajada = (progreso - 0.70f) / 0.30f;
                float pSuave = Mathf.SmoothStep(0, 1, pBajada);
                sr.transform.localPosition = Vector3.Lerp(posOriginalHijo, posSueloF3, pSuave);
            }

            // Lo volvemos transparente poco a poco
            sr.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.8f), progreso);

            // Sincronizamos la animacion para que avance hasta el 75% de este
            anim.Play(nombreAnimMuere, 0, progreso * 0.75f);
            yield return null;
        }

        // Lo dejamos congelado medio segundo
        yield return new WaitForSeconds(0.5f);

        // Bajamos el sprite a su posicion final en el suelo y se vuelve un poco mas transparente
        sr.transform.localPosition = posFinalSueloF4;
        sr.color = new Color(1, 1, 1, 0.6f);
        // Ponemos la animacion en su ultimo frame
        anim.Play(nombreAnimMuere, 0, 1f);

        // Le quitamos su collider para que Rodolfo lo atraviese
        if (col != null) col.enabled = false;

        // Resteamos las fisicas para que no se muevan y no consuma recursos
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Hacemos una pequeña pausa para que se vea bien el cuerpo
        yield return new WaitForSeconds(0.5f);

        // Lo dejamos todo congelado
        anim.speed = 0;
        sr.transform.localPosition = posFinalSueloF4;

        // Cambiamos el tag del enemigo a EnemigoMuerto para el tema del ultimo nivel
        gameObject.tag = "EnemigoMuerto";
    }
}