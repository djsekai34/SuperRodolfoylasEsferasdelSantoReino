using System.Collections;
using UnityEngine;

public class Rodolfo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float velocidadCorrer = 10f;
    public float fuerzaSalto = 5f;
    public float fuerzaSaltoShift = 8f;

    [Header("Ataque Rodolfo")]
    public GameObject bolaPrefab;
    public Transform puntoDisparo;
    public AudioClip ataqueSfx; 
    private bool estaAtacando = false;

    private Rigidbody2D fisicasJugador;
    private SpriteRenderer spriteJugador;
    private Animator animacion;
    private AudioSource audioSource;

    private float inputHorizontal;

    [Header("Configuración Transformación")]
    public LayerMask capaTransformacion;
    public float radioDeteccion = 50f;

    [Header("Efectos de Daño")]
    public float tiempoInmortal = 4f;
    private bool esInmortal = false;

    [Header("Bola en cabeza")]
    [SerializeField] private SpriteRenderer spriteCabeza;

    void Start()
    {
        fisicasJugador = GetComponent<Rigidbody2D>();
        spriteJugador = GetComponentInChildren<SpriteRenderer>();
        animacion = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Creamos un circulo invisible en Rodolfo si detecta algo en la capa Transformacion lo guardamos
        Collider2D objetoTransform = Physics2D.OverlapCircle(transform.position, radioDeteccion, capaTransformacion);

        bool hayTransformacionCerca = objetoTransform != null;

        if (Time.timeScale == 0) return; //Si el juego en pausa no leemos nada
        if (estaAtacando) return; //Si lanzamos una bola bloqueamos el moviemiento

        //Leemos las teclas de movimiento y detectamos si estamos corriendo o no
        inputHorizontal = Input.GetAxis("Horizontal");
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift);

        // Si corremos y no tenemos algo con la capa transformacion usamos la velocidad de correr si no la velocidad normal
        float velocidadActual = (estaCorriendo && !hayTransformacionCerca) ? velocidadCorrer : velocidad;

        if (fisicasJugador != null)
        {
            //Aplicamos velocidad al rigidbody pero bloqueamos la y para el tema de la gravedad
            fisicasJugador.linearVelocity = new Vector2(inputHorizontal * velocidadActual, fisicasJugador.linearVelocity.y);
        }

        //Si no hay transformacion cerca entramos al if
        if (!hayTransformacionCerca)
        {
            //Si le hemos dado a la hacemos la ecoroutina de ataque
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(SecuenciaAtaque());
            }
            //Si le hemos dado al espacio y estamos en el suelo
            if (Input.GetKeyDown(KeyCode.Space) && TocandoSuelo())
            {
                //Si le hemos dado al shift usamos el salto ponente si no el normal y le damos un empujon con el vector 2 y la fuerza que pusimos
                float fuerzaFinal = estaCorriendo ? fuerzaSaltoShift : fuerzaSalto;
                fisicasJugador.AddForce(Vector2.up * fuerzaFinal, ForceMode2D.Impulse);
            }
        }
        //Para girar el sprite y posicionar la camara
        if (inputHorizontal < -0.01f)
        {
            spriteJugador.flipX = true;

            // Si vamos a la izquierda posicionamos la camara para que se vea a la izquierda
            if (Camera.main != null)
            {
                Camera.main.transform.localPosition = new Vector3(-6.1f, 2.91f, -10f);
            }
        }
        else if (inputHorizontal > 0.01f)
        {
            spriteJugador.flipX = false;

            // Si vamos a la derecha volvemos a nuestra posicion original
            if (Camera.main != null)
            {
                Camera.main.transform.localPosition = new Vector3(6.599998f, 2.91f, -10f);
            }
        }

        //Actualizamos el estado de las animaciones
        AnimarJugador();
    }

    public void TomarDano()
    {
        // Hacemos a Rodolfo inmortal si esta haciendo su ataque o ha recibido daño o esta en el final del nivel
        if (esInmortal || estaAtacando || FinDeNivel.rodolfoEsInmortal) return;

        //Llamamos al metodo restar vida del game manager para restar la vida y iniciamos la corutina de ser inmortal 
        GameManager.RestarVida();
        StartCoroutine(SecuenciaInmortal());
    }

    IEnumerator SecuenciaInmortal()
    {
        //Activamos la inmortalidad y ponemos a Rodolfo Rojo
        esInmortal = true;
        spriteJugador.color = Color.red;
        //Hacemos a rodolfo inmortal el tiempo que deseemos
        yield return new WaitForSeconds(tiempoInmortal);
        //Lo volvemos a su color original y le quitamos la inmortalidad
        spriteJugador.color = Color.white;
        esInmortal = false;
    }

    IEnumerator SecuenciaAtaque()
    {
        //Rodolfo es inmortal
        estaAtacando = true;

        //Guardamos la gravedad que tiene rodolfo y le quitamos su gravedad y frenamos su velocidad (lo hacemos por si atacamos en el aire)
        float gravedadOriginal = fisicasJugador.gravityScale;
        fisicasJugador.gravityScale = 0;
        fisicasJugador.linearVelocity = Vector2.zero;

        if (animacion != null)
        {
            //Hacemos su animacion desde principio y calculamos cuanto dura
            animacion.Play("RoldofoShagami-do", 0, 0f);
            float duracionAnim = animacion.GetCurrentAnimatorStateInfo(0).length;
            //Esperamos a que se haga el 90% de la animacion y lo congelamos
            yield return new WaitForSeconds(duracionAnim * 0.9f);
            animacion.speed = 0;
            //Esperamos medio segundo, lanzamos la bola, ponemos un sonido, reaunamos la animacion 
            yield return new WaitForSeconds(0.5f);
            if (audioSource != null && ataqueSfx != null)
            {
                audioSource.PlayOneShot(ataqueSfx);
            }
            LanzarBola();
            animacion.speed = 1;
        }

        //Deolvemos a Rodolfo su gravedad
        fisicasJugador.gravityScale = gravedadOriginal;

        // Le damos un 0,75s mas de inmortalidad
        yield return new WaitForSeconds(0.75f);

        //Vuelve a ser mortal
        estaAtacando = false;
    }

    void LanzarBola()
    {
        //Comprobamos que hemos puesto el prefab de la bola
        if (bolaPrefab != null)
        {
            //Miramos a donde esta mirando el sprite y si hemos puesto un punto de disparo sale desde ahi si no desde el pecho
            float direccion = spriteJugador.flipX ? -1f : 1f;
            Vector3 posicionDisparo = puntoDisparo != null ? puntoDisparo.position : transform.position;

            //Si estamos girando a la izquierda, el punto debe de cruzarse al otro lado
            if (spriteJugador.flipX && puntoDisparo != null)
            {
                float offsetLocalX = puntoDisparo.localPosition.x;
                // Calculamos la posición invertida para que la bola no salga desde la espalda
                posicionDisparo = transform.position + new Vector3(-offsetLocalX, puntoDisparo.localPosition.y, 0);
            }
            //Creamos la bola
            GameObject bola = Instantiate(bolaPrefab, posicionDisparo, Quaternion.identity);

            //Le damos velocidad a la bola
            Rigidbody2D rbBola = bola.GetComponent<Rigidbody2D>();
            if (rbBola != null)
            {
                rbBola.gravityScale = 0; //no tenga gravedad
                rbBola.linearVelocity = new Vector2(direccion * 13f, 0); // Velocidad de como se movera
            }
            //Destruimos la bola al cabo de 1,5s
            Destroy(bola, 1.5f);
        }
    }

    //Comprobamos si estamos tocando el suelo o no
    private bool TocandoSuelo()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.7f);
        return hit.collider != null;
    }

    private void AnimarJugador()
    {
        //Comprobamos si falta algun componente o si estamos atacando
        if (animacion == null || fisicasJugador == null || estaAtacando) return;
        //Guardamos la velocidad actual para saber que hace el cuerpo de Rodolfo
        float velX = fisicasJugador.linearVelocity.x;
        float velY = fisicasJugador.linearVelocity.y;


        //1º Comprobacion, si el Tocando suelo no detecta o la velocidad del Y es salta
        if (!TocandoSuelo() || Mathf.Abs(velY) > 0.1f)
        {
            // Si no se está reproduciendo ya la animacion correspondiente nosotros se la ponemos
            if (!animacion.GetCurrentAnimatorStateInfo(0).IsName("RodolfoSalta"))
                animacion.Play("RodolfoSalta");
        }
        //Estamos en el suelo y usamos Mathf.Abs para que el valor es positivo da igual la direccion
        else if (Mathf.Abs(velX) > 0.1f)
        {
            if (!animacion.GetCurrentAnimatorStateInfo(0).IsName("RodolfoCorre"))
                animacion.Play("RodolfoCorre");
        }
        //Si no esta en el aire y esta en el suelo es que esta quieto
        else
        {
            if (!animacion.GetCurrentAnimatorStateInfo(0).IsName("IdleRodolfo"))
                animacion.Play("IdleRodolfo");
        }
    }

    public void ActualizarBolaCabeza(Sprite nuevoSpriteBola)
    {
        if (spriteCabeza != null)
        {
            spriteCabeza.sprite = nuevoSpriteBola;

            spriteCabeza.transform.localPosition = new Vector3(
            spriteCabeza.transform.localPosition.x,
            spriteCabeza.transform.localPosition.y,
            spriteCabeza.transform.localPosition.z
            );
        }
    }
}