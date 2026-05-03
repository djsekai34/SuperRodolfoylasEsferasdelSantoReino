using System.Collections;
using UnityEngine;

public class Kefla : MonoBehaviour
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

    [Header("Ataque en Ráfaga")]
    public GameObject Bola_Kefla;
    public float cadenciaRafaga = 0.5f;

    private Vector3 posicionInicial;
    private Vector3 objetivoActual;
    private Animator anim;
    private Coroutine corrutinaAtaque;

    void Start()
    {
        // Almacenamos la coordenada de donde aparece el enemigo, establecemos el primer destino y buscamos el animator en el hijo
        posicionInicial = transform.position;
        objetivoActual = puntoDestinoXYZ;
        anim = GetComponentInChildren<Animator>();

        // Si existe el animator emepzamos por defecton con la de moverse
        if (anim != null && !estaAtacando)
        {
            anim.Play("KeflaMoviendose");
        }
    }

    void Update()
    {
        // Si esta muerto nos da igual todo lo demas
        if (estaMuerto) return;

        // Llamamos todo el rato al meotod que calcula la distacia y vision del enemigo al jugador
        DetectarJugador();

        if (anim != null)
        {
            // Mandamos el estado de estaAtacando al animator controler asi sabe si atacar o caminar
            anim.SetBool("Atacando", estaAtacando);
        }

        // Si el enemigo no esta atacando, el enemigo se mueve
        if (!estaAtacando)
        {
            MoverEnemigo();
        }
    }

    void DetectarJugador()
    {
        // Si no hemos asignado el punto de donde sale el rayo paramos todo para no tener errores
        if (puntoDeEmision == null) return;

        // Calculamos la direccion del rayo hacia la izquierda y lo lanzamos para que solo detecte la capa del jugador
        Vector3 direccionRayo = -transform.right;
        RaycastHit2D choque = Physics2D.Raycast(puntoDeEmision.position, direccionRayo, distanciaRaycast, capaJugador);

        // Si el rayo toca algo y tiene el tag Player
        if (choque.collider != null && choque.collider.CompareTag("Player"))
        {
            // Si no esta atacando iniciamos el ataque
            if (!estaAtacando)
            {
                estaAtacando = true;

                if (anim != null)
                {
                    // Nos aseguramos que la animacion no este pausada y la forzamos que inicie desde el principio
                    anim.speed = 1f;
                    anim.Play("KeflaAtaque", 0, 0f);
                }

                // Si ya estabamos atacando de antes, lo matamos para que no se solapen los disparos
                if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);
                // Iniciamos la rafaga de ataques y lo guardamos
                corrutinaAtaque = StartCoroutine(RafagaDeBolas());
            }
        }
        else
        {
            // Si el jugador ya no lo tenemos delante y estabamos atacando
            if (estaAtacando)
            {
                // Paramos de atacar
                estaAtacando = false;

                if (anim != null)
                {
                    anim.speed = 1f;
                    // Volvemos a caminar
                    anim.Play("KeflaMoviendose");
                }
                // Cancelamos la rafaga de ataque
                if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);
            }
        }
    }

    IEnumerator RafagaDeBolas()
    {
        if (anim != null)
        {
            // Restauramos la velocidad normal y Forzamos la animacion de ataque
            anim.speed = 1f;
            anim.Play("KeflaAtaque", 0, 0f);

            // Esperamos de forma dinamica hasta que el Animator haya entrado a la animcion de ataque
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("KeflaAtaque"));

            // Calculamos la duracion real de la animacion
            float duracion = anim.GetCurrentAnimatorStateInfo(0).length;

            // Espertamos al 90% de la animacion para llegar al frame que deseamos
            yield return new WaitForSeconds(duracion * 0.9f);

            // Congelamos el dibujo en el frame que deseamos
            anim.Play("KeflaAtaque", 0, 0.99f);
            anim.Update(0);
            anim.speed = 0f;

            // Esperamos un poquillo y lanzamos la bola
            yield return new WaitForSeconds(0.05f);
            LanzarBola();
        }

        // Mientras el rayo detecte al jugador y no haya muerto, seguimos disparando.
        while (estaAtacando && !estaMuerto)
        {
            // Esperamos un tiempo entre disparos que es el que le hayamos puesto para volver a disparar
            // comprobamos si seguimos en rango de ataque para disparar (por si se mueve el jugador)
            yield return new WaitForSeconds(cadenciaRafaga);
            if (estaAtacando) LanzarBola();
        }
    }

    // Lanzamos la bola
    void LanzarBola()
    {
        if (Bola_Kefla != null)
        {
            Instantiate(Bola_Kefla, puntoDeEmision.position, transform.rotation);
        }
    }

    // Movemos el enemigo
    void MoverEnemigo()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);

        // Aseguarmos que el movimiento se haga con la animacion
        if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("KeflaMoviendose"))
        {
            anim.Play("KeflaMoviendose");
        }

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
    // --- SECCIÓN DE MUERTE: SE QUEDA EN EL SITIO ---
    public void Morir()
    {
        // Si estamos muerto nos salimos para no ejecutar varias veces lo mismo
        if (estaMuerto) return;
        estaMuerto = true;
        estaAtacando = false;

        // Detenemos totalmente cualquier rafaga de disparo
        if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);

        // Desactivamos las fisicas del padre para que no bloquee la caida del sprite
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        // Cogemos el collider y de solido lo pasamos a fantasma para atravesarlo
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (anim != null)
        {
            // Reducimos la velocidad al 40%, inicamos su animacion de muerto y llamamos a la corrutina del efecto estatua
            anim.speed = 0.4f;
            anim.Play("KeflaMuriendose");
            StartCoroutine(EfectoEstatua());
        }
    }

    IEnumerator EfectoEstatua()
    {
        // Obtenemos el sprite del hijo y si no hubiera nos salimos para no tener problemas
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;

        // Guardamos la posicion relativa inicial que tenemos del sprite
        Vector3 posicionAire = sr.transform.localPosition;

        // Hacemos una pequeña pausa con vibracion
        float tiempoDeEspera = 1.2f;
        float cronometro = 0;

        // Mienras el tiempo trascurrido sea mehnor que la duracion total que hayamos planeado
        while (cronometro < tiempoDeEspera)
        {
            // Incrementamos el contador basandonos en le tiempo real ocurrido entre frames
            cronometro += Time.deltaTime;

            // Normalizamos el tiempo entre un porcentaje de 0 a 1
            float progreso = cronometro / tiempoDeEspera;

            if (anim != null)
            {
                // Reproducimos solo el 65% de la animacion de muerte
                anim.Play("KeflaMuriendose", 0, progreso * 0.65f);
            }

            // Cuando queda poco tiempo el sprite empieza a vibrar
            if (progreso > 0.6f)
            {
                float fuerzaTemblor = 0.04f; // La fuerza que usamos
                // Desplazamos la poscion del sprite local usando un punto aleatorio en un circulo
                sr.transform.localPosition = posicionAire + (Vector3)Random.insideUnitCircle * fuerzaTemblor;
            }
            else
            {
                // Si no hemos llegado al momento del temblor dejamos el sprite donde esta inicialmente
                sr.transform.localPosition = posicionAire;
            }

            // Pausamos el frame en concreto
            yield return null;
        }

        // Laznamos un raycast hacia el suelo para saber a que distancia estamos
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 15f, LayerMask.GetMask("Suelo"));
        float distanciaAlSuelo = 0f;

        // ¿Hemos golepado a algo que tiene collider?
        if (hit.collider != null)
        {
            // Calculamos la distancia del centro del spirte y dejamos un margen extra
            distanciaAlSuelo = (hit.distance - sr.bounds.extents.y) + 0.35f;
        }

        // Definimos el destino final que ira el sprite
        Vector3 posFinalHijo = posicionAire - new Vector3(0, distanciaAlSuelo, 0);

        // Ponemos fluidez a la caida
        float tiempoCaida = 0;
        float duracionCaida = 0.6f;

        // Mientras el tiempo que haya transcurrido sea menos que de la duracion que hemos planeado
        while (tiempoCaida < duracionCaida)
        {
            // Acumulamos el tiempo real para que la caida sea fluida
            tiempoCaida += Time.deltaTime;

            // Normalizamos el tiempo entre un porcentaje de 0 a 1
            float t = tiempoCaida / duracionCaida;

            // Aplicamos una formula SMOOTHSTEP para que la caida del sprite sea suave
            // La fórmula es: t * t * (3f - 2f * t)
            float progresoSuave = t * t * (3f - 2f * t);

            // Movemos el sprite de donde este al suelo de forma suave
            sr.transform.localPosition = Vector3.Lerp(posicionAire, posFinalHijo, progresoSuave);

            // Cambiamos el color del sprite a grisaceo y le bajamos la opacidad
            sr.color = Color.Lerp(Color.white, new Color(0.5f, 0.5f, 0.5f, 0.8f), progresoSuave);

            if (anim != null)
            {
                // Mientras que se caiga el sprite lo dejamos clavado en el ultimo sprite
                anim.Play("KeflaMuriendose", 0, 0.99f);
            }

            yield return null;
        }

        // Aseguramos la posicion del sprite y lo congelamos
        sr.transform.localPosition = posFinalHijo;
        if (anim != null) anim.speed = 0;

        // Cambiamos el tag del enemigo a EnemigoMuerto para el tema del ultimo nivel
        gameObject.tag = "EnemigoMuerto";
    }
}