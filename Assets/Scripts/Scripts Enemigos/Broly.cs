using System.Collections;
using UnityEngine;

public class Broly : MonoBehaviour
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
    public GameObject Bola_Broly;
    public float cadenciaRafaga = 0.5f;

    private Vector3 posicionInicial;
    private Vector3 objetivoActual;
    private Animator anim;
    private Coroutine corrutinaAtaque;

    void Start()
    {
        //Guardamos el punto exacto donde ponemos a broly en el mapa
        posicionInicial = transform.position;
        //Decidimos su primer objetivo osea el punto b que le pongamos
        objetivoActual = puntoDestinoXYZ;
        //Buscamos el animator que estara en un objeto hijo
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //Si broly ha muerto no salidos del codigo ya que no tiene sentido patrullar
        if (estaMuerto) return;

        // Vigilamos cada frame
        DetectarJugador();

        //Le pasamos al animator como esta la variable estaAtacando, para ver si esta atacando o caminando 
        if (anim != null)
        {
            anim.SetBool("Atacando", estaAtacando);
        }

        //Movemos a Broly si este no esta atacando.
        if (!estaAtacando)
        {
            MoverEnemigo();
        }
    }

    void DetectarJugador()
    {
        //Si no hemos puesto el punto de donde salga, pues no hacemos nada
        if (puntoDeEmision == null) return;

        //Lanzamos el rayo a la izquierda y le ponemnos la distancia de tan lejos que ve y la capa que debe de hacer caso y le de igual todo
        Vector3 direccionRayo = -transform.right;
        RaycastHit2D choque = Physics2D.Raycast(puntoDeEmision.position, direccionRayo, distanciaRaycast, capaJugador);

        //El rayo ha tocado a Rodolfo?
        if (choque.collider != null && choque.collider.CompareTag("Player"))
        {
            //Entramos al if si no estabamos atacando para no repetir todo lo del inicio
            if (!estaAtacando)
            {
                estaAtacando = true;

                // Forzamos a que la animación de ataque empiece desde el principio siempre
                if (anim != null)
                {
                    anim.speed = 1f;
                    anim.Play("BrolyAtaque", 0, 0f);
                }
                // Si nos quedamose a medias, pues lo paramis todo y volvemos a emepzar
                if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);
                corrutinaAtaque = StartCoroutine(RafagaDeBolas());
            }
        }
        else
        {
            //Si Rodolfo se escapa o se ha escondido, volvemos a ponerlo a andar y que deje de lanzar bolas
            if (estaAtacando)
            {
                estaAtacando = false;
                if (anim != null) anim.speed = 1f;
                if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);
            }
        }
    }

    IEnumerator RafagaDeBolas()
    {
        if (anim != null)
        {
            // Empezamos la animacion de ataque
            anim.speed = 1f;
            anim.Play("BrolyAtaque", 0, 0f);

            // Esperamos a que la aniamcion llege al ultimo frame y detectamos que estamos en la animacion de ataque
            float duracion = 0;
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("BrolyAtaque"));


            // Calculamos cuanto dura la animacion y lo guardamos
            duracion = anim.GetCurrentAnimatorStateInfo(0).length;

            // Esperamos a que se reproduzca el 90% de la animacion
            yield return new WaitForSeconds(duracion * 0.9f);

            // Forzamos a que el animator se pare en un sitio concreto de la animacion que queramos,
            // como en mi caso es en el ultimo frame ponemos 0.99f en vez de 1.0f para que no salte a la siguiente animacion
            anim.Play("BrolyAtaque", 0, 0.99f);

            // Obligamos al animator a que refresque su estado interno para que se quede en el ultimo frame
            anim.Update(0);

            // Congelamos a Broly en seco
            anim.speed = 0f;

            // Esperamos un chispo para lanzar la bola y llamamos a la funcion para lanzar la bola
            yield return new WaitForSeconds(0.05f);

            LanzarBola();
        }

        // Bucle de ráfaga
        while (estaAtacando && !estaMuerto)
        {
            yield return new WaitForSeconds(cadenciaRafaga);
            if (estaAtacando) LanzarBola();
        }
    }

    //Creamos la bola desde el punto y rotacion que hayamis puesto
    void LanzarBola()
    {
        if (Bola_Broly != null)
        {
            Instantiate(Bola_Broly, puntoDeEmision.position, transform.rotation);
        }
    }

    // Movemos el enemigo desde un punto a hacia un punto b yendo y vieniendo (Como con freezer)
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
        //Si esta muerto no debe de morir otra vez
        if (estaMuerto) return;
        //Decimos que esta muerto y que deje de atacar
        estaMuerto = true;
        estaAtacando = false;

        //Paramos la corrutina de golpe para que no siga sacando bolas
        if (corrutinaAtaque != null) StopCoroutine(corrutinaAtaque);

        //Buscamos el rigidbody de broly
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            //Hacemos que broly salga del mundo fisico
            rb.simulated = false;
        }

        // Buscamos el collider de este
        Collider2D col = GetComponent<Collider2D>();

        if (col != null) col.isTrigger = true; //Hacemos que rodolfo lo pueda atravesar
        //Si broly tiene la animacion de morir se la reproducimos, en una velocidad mas lenta y lanzamos la corrutina para que sea gris
        if (anim != null)
        {
            anim.speed = 0.4f;
            anim.Play("BrolyMuere");
            StartCoroutine(EfectoEstatua());
        }
    }

    IEnumerator EfectoEstatua()
    {

        //Buscamos el sprite de broly y si hubiera algun error cortamos ahi todo
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;

        float distanciaAlSuelo = 1.5f; //Vairalbe que usaremos para calcular la distancia huaca el suelo

        //Lanzamos un raycast hacia abajo para saber donde esta el suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 15f, LayerMask.GetMask("Ground"));
        //Si el rayo toca el suelo la distancia que hay hacia el
        if (hit.collider != null) distanciaAlSuelo = hit.distance;

        //Guardamos el cuerpo donde esta y calculamos donde debe de acabar
        Vector3 posInicialHijo = sr.transform.localPosition;
        Vector3 posFinalHijo = posInicialHijo - new Vector3(0, distanciaAlSuelo, 0);

        float tiempoCaida = 0;
        float duracionCaida = 1.5f; // El tiempo que tarda en caer al suelo

        //Congelamos la animacion, para que nosotros lo manipulemos como deseemos
        if (anim != null) anim.speed = 0;

        //Bucle de que se cae
        while (tiempoCaida < duracionCaida)
        {
            //Empezamos a contar y calculamos el progreso
            tiempoCaida += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempoCaida / duracionCaida);

            // Movemos el sprite de broly poco a poco hacia abajo
            sr.transform.localPosition = Vector3.Lerp(posInicialHijo, posFinalHijo, progreso);

            // Lo vamos volviendo invisble
            sr.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.5f), progreso);

            //Forzamos a que la animacion de broly vaya en conjunto por como se vaya cayendo
            if (anim != null)
            {
                anim.Play("BrolyMuere", 0, progreso);
            }
            //Nos espertamos al siguiente frame para moverlos
            yield return null;
        }

        if (anim != null)
        {
            //Nos aseguramos que el ultimo frame sea el que queremso de la muerte y lo dejamos parado
            anim.Play("BrolyMuere", 0, 1.0f);
            anim.speed = 0;
        }

        // Cambiamos el tag del enemigo a EnemigoMuerto para el tema del ultimo nivel
        gameObject.tag = "EnemigoMuerto";
    }
}