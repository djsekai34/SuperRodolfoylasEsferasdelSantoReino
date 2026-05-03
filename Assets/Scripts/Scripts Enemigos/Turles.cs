using System.Collections;
using UnityEngine;

public class Turles : MonoBehaviour
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
    public GameObject Bola_Turles;
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
            // Ponemos la velovidad normal reseteamos lo de ataque y movemos al enemigo
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
        // Verificamos si existe el animator
        if (anim != null)
        {
            // Inicamos la animacion de ataque desde el principio
            anim.speed = 1f;
            string nombreAnim = "TurlesAtaque";
            anim.Play(nombreAnim, 0, 0f);

            // Damos un pequeño respiro para que unity procese el cambio
            yield return new WaitForSeconds(0.1f);

            // Obtenemos lo que duracion real de la animacion en clip de animacion y por seguridad si no lo detecta le ponemos 1s
            float duracion = anim.GetCurrentAnimatorStateInfo(0).length;
            if (duracion <= 0) duracion = 1f;

            // Esperamos a que llegue hasya casi al final de la animacion
            yield return new WaitForSeconds(duracion * 0.9f);

            // Congelamos la animacion en el ultimo frame
            anim.Play(nombreAnim, 0, 0.99f);
            anim.Update(0);
            anim.speed = 0f;
        }

        // Lanzamos un ataque
        LanzarDisco();

        // Mientras detectemos a rodolfo y no haya muerto el enemigo no paramos de disparar
        while (estaAtacando && !estaMuerto)
        {
            // Esperamos segun la cadencia de ataque que hayamos puesto
            yield return new WaitForSeconds(cadenciaAtaque);

            // Comprobamos por ultima vez por si ha muerto y ya disparamos
            if (estaAtacando && !estaMuerto)
            {
                LanzarDisco();
            }
        }
    }

    // Lanzamos la bola
    void LanzarDisco()
    {
        if (Bola_Turles != null)
            Instantiate(Bola_Turles, puntoDeEmision.position, transform.rotation);
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
        // Obtemeos el sprite del hijo para poder moverlo sin que afecte al padre y si no hay nada nos salimos
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;

        // Lanzamos un raycast hacia abajo para calcular con exactitud la distancia hacia el suelo
        float distanciaAlSueloFinal = 1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 15f, LayerMask.GetMask("Ground"));
        // Si el rayo toca el suelo se actualiza la distancia
        if (hit.collider != null) distanciaAlSueloFinal = hit.distance;

        // Guardamos la posicion original
        Vector3 posOriginalHijo = sr.transform.localPosition;

        // Calculamos dos destinos diferentes para el frame 3 y el frame 4 uno mas arriba que el otro
        float margenF3 = 0.7f;
        Vector3 posSueloF3 = posOriginalHijo - new Vector3(0, distanciaAlSueloFinal - margenF3, 0);

        float margenF4 = 0.4f;
        Vector3 posSueloF4 = posOriginalHijo - new Vector3(0, distanciaAlSueloFinal - margenF4, 0);

        // Pausamos la velocidad normnal del animator porque lo vamos a controlar nosotros y ponemos el nombre de la animacion para morir
        if (anim != null) anim.speed = 0;
        string nombreAnimMorir = "TurlesMuere";

        // Tenemos dos varaibles una para calcular el tiempo de como ira la animacion y otra lo que queremos que dure en total
        float tiempo = 0;
        float duracionTotal = 2.5f;

        // Si el tiempo es menor a la duracion total entramos
        while (tiempo < duracionTotal)
        {
            // Acumulamos el tiempo y normalizamos la animacion
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionTotal;

            // Bloqueamos la animacion hasta el 75% en su posicion original 
            if (progreso < 0.75f)
            {
                sr.transform.localPosition = posOriginalHijo;
            }
            else
            {
                // En el 25% restante bajamos de forma suave
                float pBajada = (progreso - 0.75f) / 0.25f;
                float pSuave = Mathf.SmoothStep(0, 1, pBajada);
                sr.transform.localPosition = Vector3.Lerp(posOriginalHijo, posSueloF3, pSuave);
            }

            // Se va volviendo transparente
            sr.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.9f), progreso);

            // Volvemos a sincronizar el frame de la animacion con el tiempo que ha transcurrido y no salimos
            anim.Play(nombreAnimMorir, 0, progreso * 0.66f);
            yield return null;
        }

        // Forzamos la posicon y lo deamos ahi parado 1s en el 3 frame
        sr.transform.localPosition = posSueloF3;
        sr.color = new Color(1, 1, 1, 0.7f);
        anim.Play(nombreAnimMorir, 0, 0.66f);

        // Ponemos el collider trigger para que se pueda atravesar
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Esperamos un poco para que se vea bien la animacion del 3 frame
        yield return new WaitForSeconds(1f);

        // Forzamos la posicon y lo deamos ahi parado 1s en el 4 frame
        sr.transform.localPosition = posSueloF4;
        sr.color = new Color(1, 1, 1, 0.6f);
        anim.Play(nombreAnimMorir, 0, 1.0f);

        // Esperamos un poco para que veamos bien el cuerpo en el suelo
        yield return new WaitForSeconds(0.3f);

        // Lo congelamos de forma definitiva
        anim.speed = 0;
        sr.transform.localPosition = posSueloF4;

        // Cambiamos el tag del enemigo a EnemigoMuerto para el tema del ultimo nivel
        gameObject.tag = "EnemigoMuerto";
    }
}