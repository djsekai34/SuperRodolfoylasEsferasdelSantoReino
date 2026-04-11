using System.Collections;
using UnityEngine;

public class FrezzerTransformacion : MonoBehaviour
{
    [Header("Detección")]
    public Transform puntoX;
    private float alcance = 10.5f;
    public string tagObjetivo = "Player";

    [Header("Referencias")]
    public Animator animadorHijo;
    public GameObject objetoAEliminar;

    [Header("Ajustes de camara para la transformacion")]
    private float intensidadAgite = 0.20f;
    private float tiempoAnimacion = 2.77f;
    private float tiempoPostEspera = 1.5f;

    private Camera camaraPrincipal;
    private Vector3 posicionOriginalCamara;
    private bool activado = false;

    void Start()
    {
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        // Si el rayo nos ha visto o no tiene el punto de origen configurado no hacemos nada
        if (activado || puntoX == null) return;

        //Calulamos el rayo por donde sale, hacia donde mira y calculamos donde termina el rayo
        Vector2 origen = puntoX.position;
        Vector2 direccion = -puntoX.right;
        Vector3 puntoFinal = (Vector3)origen + (Vector3)(direccion * alcance);

        //Hacemos que el rayo ignore la capa de Transformacion y con el ~ hacemos que detecte todo menos lo que tenga eso
        int mascaraIgnorar = LayerMask.GetMask("Transformacion");
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, alcance, ~mascaraIgnorar);

        //Si el rayo toca algo y ese algo tiene el tag objetivo que deseemos hara la corutina
        if (hit.collider != null && hit.collider.CompareTag(tagObjetivo))
        {
            StartCoroutine(SecuenciaFinal());
        }
    }

    IEnumerator SecuenciaFinal()
    {
        // Iniciamos el evento donde bloqueamos el update y guardamos la camara
        activado = true;
        posicionOriginalCamara = camaraPrincipal.transform.localPosition;

        // Congelamos el moviento de todo y iniciamos la animacion
        Time.timeScale = 0f;
        animadorHijo.SetTrigger("TransformacionFreezer");

        // Hacemos un temblor en la camara
        float transcurrido = 0f;
        while (transcurrido < tiempoAnimacion)
        {
            //Generamos un movimiento de camara aleatorio
            float x = Random.Range(-1f, 1f) * intensidadAgite;
            float y = Random.Range(-1f, 1f) * intensidadAgite;

            // Movemos un poco la camara de su sitio original
            camaraPrincipal.transform.localPosition = new Vector3(posicionOriginalCamara.x + x, posicionOriginalCamara.y + y, posicionOriginalCamara.z);
            //Ponemos un cronometro con unscaledDeltaTime que es idependiente si el juego esta congelado o no
            transcurrido += Time.unscaledDeltaTime;
            //Esperamos al siguiente frame
            yield return null;
        }

        //Devolvemos la camara a su sitio y usamos WaitForSecondsRealtime porque seguimso con el juego congelado y asi lo dejamos en el ultimo frame de la animacion y vemos la transformacion
        camaraPrincipal.transform.localPosition = posicionOriginalCamara;
        yield return new WaitForSecondsRealtime(tiempoPostEspera);

        //Volvemos al juego normal y eliminamos el objeto
        Time.timeScale = 1f;
        Destroy(objetoAEliminar);
    }
}