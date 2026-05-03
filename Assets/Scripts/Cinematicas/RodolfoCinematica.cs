using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para detectar la escena actual

public class RodolfoCinematica : MonoBehaviour
{
    private Animator anim;
    private GameObject bolaRodolfo;
    private Vector3 posicionOriginalBola;
    private Transform padreOriginalBola;
    private Vector3 escalaOriginalPersonaje;
    private Vector3 posicionOriginalPersonaje;

    [Header("Configuración Animación")]
    public string nombreAnimacion = "Shagamido";
    public string estadoIdle = "Idle";

    [Header("Ajustes del Proyectil")]
    public float velocidadDerecha = 50f;
    public float tiempoVida = 1.8f;

    private Vector3 escalaSagradaBola = new Vector3(0.9965155f, 0.6397434f, 1.2475f);
    private bool disparada = false;
    private Coroutine bucleActual;

    void Awake()
    {
        // Guardamos el animator y la escala con posicion inicial
        anim = GetComponent<Animator>();
        escalaOriginalPersonaje = transform.localScale;
        posicionOriginalPersonaje = transform.localPosition;

        // Buscamos entre todo los hijos hasta encontrar la bola
        foreach (Transform hijo in transform)
        {
            // Si tenemos un objeto hijo con el tag deseado
            if (hijo.CompareTag("BolaRodolfoCinematica"))
            {
                // Guardamos la bola, su padre y su posicion relativa, tras hacer esto nos salimos
                bolaRodolfo = hijo.gameObject;
                padreOriginalBola = transform;
                posicionOriginalBola = bolaRodolfo.transform.localPosition;
                break;
            }
        }
    }

    // Usamos el OnEnable porque por defecto Rodolfo esta oculto
    void OnEnable()
    {
        // Si por algun motivo ya habia algun bucle haciendsoe se corta, esto lo hacemos para que no se duplique
        if (bucleActual != null) StopCoroutine(bucleActual);

        // Resetamos el disparo y nos aseguramos que la bola este oculta por defecto
        disparada = false;
        if (bolaRodolfo != null) bolaRodolfo.SetActive(false);

        // Arrancamos la corrutina y la guardamos
        bucleActual = StartCoroutine(BucleAtaqueInfinito());
    }

    IEnumerator BucleAtaqueInfinito()
    {
        // Configuramos las posiciones predeterminadas
        float posXImpacto = -31f;
        float posXSalidaBola = 148f; 

        // Si estamos en esta escena sale desde un punto en concreto que deseemos
        if (SceneManager.GetActiveScene().name == "CinematicaFinalMalo")
        {
            posXImpacto = 91f;
            posXSalidaBola = 273f; 
        }

        // Hacemos un bucle while para que Rodolfo ataque siempre
        while (true)
        {
            // La bola deja de moverse y restauramos la velocidad normal y ponemos a Rodolfo en su Idle
            disparada = false;
            if (anim != null)
            {
                anim.speed = 1;
                anim.Play(estadoIdle);
            }

            // Devolvemos a Rodolfo a su tamaño y posición original 
            transform.localScale = escalaOriginalPersonaje;
            transform.localPosition = posicionOriginalPersonaje;

            // Si existe la bola, la ocultamos volviendo a su padre con su posicion original y escala original
            if (bolaRodolfo != null)
            {
                bolaRodolfo.SetActive(false);
                bolaRodolfo.transform.SetParent(padreOriginalBola);
                bolaRodolfo.transform.localPosition = posicionOriginalBola;
                bolaRodolfo.transform.localScale = escalaSagradaBola;
            }

            // Esperamos 1s
            yield return new WaitForSeconds(1f);

            // Emepzamos la fase del shagamido
            if (anim != null)
            {
                // Lazamos la animacion del shagamido y esperamos un poco para que vea Unity que esta haciendo
                anim.Play(nombreAnimacion);
                yield return new WaitForEndOfFrame();

                // Calculamos lo que dura la animacion exatacmente para esperar el tiempo justo
                float duracionTotal = anim.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(duracionTotal);

                // Congelamos a Rodolfo en el ultimo frame
                anim.speed = 0;
            }

            // Le aplicamos a rodolfo una nueva escala y una nueva posicion en el ultimo frame
            transform.localScale = new Vector3(2.6627f, escalaOriginalPersonaje.y, escalaOriginalPersonaje.z);
            transform.localPosition = new Vector3(posXImpacto, transform.localPosition.y, transform.localPosition.z);

            if (bolaRodolfo != null)
            {
                // Sacamos la bola del padre y lo hacemos principal
                bolaRodolfo.transform.SetParent(transform.parent);

                // Situamos la bola en el punto de salida adeucado y con su escala original y que se mueva
                bolaRodolfo.transform.localPosition = new Vector3(posXSalidaBola, bolaRodolfo.transform.localPosition.y, bolaRodolfo.transform.localPosition.z);
                bolaRodolfo.transform.localScale = escalaSagradaBola;

                // La bola aparece, lo ponemos por encima de todo en la jerarquia y hacemos que se mueva
                bolaRodolfo.SetActive(true);
                bolaRodolfo.transform.SetAsLastSibling(); // EL SetAsLastSibling sirve para que sea el Nº1 en el canvas
                disparada = true;

                // La bola vuela durante el tiempo de vida asignado
                yield return new WaitForSeconds(tiempoVida);

                // Al teminar su vida lo ocultamos y lo reseteamos
                disparada = false;
                bolaRodolfo.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (disparada && bolaRodolfo != null)
        {
            bolaRodolfo.transform.Translate(Vector2.right * velocidadDerecha * Time.deltaTime);
        }
    }
}