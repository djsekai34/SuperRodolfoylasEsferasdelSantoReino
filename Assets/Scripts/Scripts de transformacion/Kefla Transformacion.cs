using System.Collections;
using UnityEngine;

public class KeflaTransformacion : MonoBehaviour
{
    [Header("Detección")]
    public Transform puntoX;
    private float alcance = 10.5f;
    public string tagObjetivo = "Player";

    [Header("Referencias")]
    public Animator animadorHijo;
    public GameObject objetoAEliminar;

    [Header("Ajustes de dramatismo")]
    public float intensidadAgite = 0.35f;
    public float tiempoAnimacion = 2.10f;
    public float tiempoPostEspera = 2.0f;

    private Camera camaraPrincipal;
    private Vector3 posicionOriginalCamara;
    private bool activado = false;

    void Start()
    {
        // Guardamos la camara
        camaraPrincipal = Camera.main;

        // Verificamos si el animator existe
        if (animadorHijo != null)
        {
            // Configuramos para que el animator ignore el tiempo de juego
            animadorHijo.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    void Update()
    {

        // Configuramos para que solo lance rayos necesarios y asi optimizar el juego
        if (activado || puntoX == null) return;

        // Definimos el punto de origen y la direccion del rayo usamos el -puntoX.right para que vaya hacia la izquierda
        Vector2 origen = puntoX.position;
        Vector2 direccion = -puntoX.right;

        // Creamos una mascara para que el rayo ignore la capa Transformacion
        int mascaraIgnorar = LayerMask.GetMask("Transformacion");

        // Lanzamos el raycast para que detecte todo menos lo que hay en la mascaraIgnorar gracias a ~
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, alcance, ~mascaraIgnorar);

        // Si el rayo choca con algo 
        if (hit.collider != null && hit.collider.CompareTag(tagObjetivo))
        {
            StartCoroutine(SecuenciaFinal());
        }
    }

    IEnumerator SecuenciaFinal()
    {
        // Bloqueamos la ejeciucion para que no se lance dos veces y guardamos la poscion de la camara
        activado = true;
        posicionOriginalCamara = camaraPrincipal.transform.localPosition;

        // Congelamos el tiempo
        Time.timeScale = 0f;

        // Si existe el animador 
        if (animadorHijo != null)
        {
            // Forzamos el inicio de la animacion y tiramos el trigger para que ocurra 
            animadorHijo.Play("KeflaTransformacion", 0, 0f);
            animadorHijo.SetTrigger("TransformacionKefla");
        }

        // EL bucle de la agitacion de la camara como ya tenemos en Freezer, Broly....
        float transcurrido = 0f;
        while (transcurrido < tiempoAnimacion)
        {
            // Usamos una semilla fija de tiempo real para que el Random sea más suave
            float x = Random.Range(-1f, 1f) * intensidadAgite;
            float y = Random.Range(-1f, 1f) * intensidadAgite;

            camaraPrincipal.transform.localPosition = new Vector3(
                posicionOriginalCamara.x + x,
                posicionOriginalCamara.y + y,
                posicionOriginalCamara.z
            );

            transcurrido += Time.unscaledDeltaTime;
            yield return null;
        }

        // Devolvemos la camara a su posiciuon original
        camaraPrincipal.transform.localPosition = posicionOriginalCamara;

        // Pausamos el tiempo que hayamos puesto
        yield return new WaitForSecondsRealtime(tiempoPostEspera);

        // Volvemos a la normalidad y borramos al enemigo
        Time.timeScale = 1f;

        if (objetoAEliminar != null)
        {
            Destroy(objetoAEliminar);
        }
    }
}