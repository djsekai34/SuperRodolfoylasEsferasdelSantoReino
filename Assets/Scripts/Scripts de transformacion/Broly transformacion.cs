using System.Collections;
using UnityEngine;

public class BrolyTransformacion : MonoBehaviour
{
    [Header("Detección")]
    public Transform puntoX;
    private float alcance = 10.5f;
    public string tagObjetivo = "Player";

    [Header("Referencias")]
    public Animator animadorHijo;
    public GameObject objetoAEliminar;

    [Header("Ajustes de camara para la transformacion")]
    private float intensidadAgite = 0.3f;
    private float tiempoAnimacion = 8.2f;
    private float tiempoPostEspera = 1.5f;

    [Header("Efecto de Pausa Dramática")]
    private float tiempoDePausaGrito = 0.6f;

    private Camera camaraPrincipal;
    private Vector3 posicionOriginalCamara;
    private bool activado = false;

    void Start()
    {
        //Buscamos la camara que tenga el tag main camara y la guardamos para usarla en el futuro
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        //Si ya esta activa la secuencia o si no hemos puesto un punto de origen, no leemos nada
        if (activado || puntoX == null) return;

        //Guardamos el origen y direccion y direccion para el raycast
        Vector2 origen = puntoX.position;
        Vector2 direccion = -puntoX.right;

        //Creamos una mascara para que el raycast no choque con lo que tenga Transforemacion
        int mascaraIgnorar = LayerMask.GetMask("Transformacion");
        //Generamos el raycast añadiendo el ~ para que detecte todo menos lo que est aen la variavle mascaraignorar
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, alcance, ~mascaraIgnorar);

        //Si chocamos con algo y tiene el tag objetivo emepzamos la corrutina
        if (hit.collider != null && hit.collider.CompareTag(tagObjetivo))
        {
            StartCoroutine(SecuenciaFinal());
        }
    }

    IEnumerator SecuenciaFinal()
    {
        //Marcamos que la secuencia ha empezamos y guardamos la camara
        activado = true;
        posicionOriginalCamara = camaraPrincipal.transform.localPosition;

        //Congelamos todo
        Time.timeScale = 0f;

        if (animadorHijo != null)
        {
            //Forzamos a que se haga la animacion se haga desde el segundo 0 siempre asi no dejamos la animacion a medias etc
            animadorHijo.Play("BrolyTransformacion", 0, 0f);
            //Hacemos que la animacion se mueva normal
            animadorHijo.speed = 1f;
            // Actiavamos el trigger para que el animator sepa que nos estamos transformando
            animadorHijo.SetTrigger("BrolyTransformacion");
        }

        // Creamos unas variables para controlar el timing para el frame 4
        float tiempoFrame4 = 1.22f; // El momento exacto donde broly grita
        float tiempoParaEmpezarAgite = 1.6f; // El momento donde todo tiembla
        float transcurrido = 0f; // Cronometro para calcular todo
        bool pausaHecha = false; // Interruptor

        //Bucle de la cinematrica
        while (transcurrido < tiempoAnimacion)
        {
            // 1 Apartado: Logica para la pausa del grito de broly
            if (!pausaHecha && transcurrido >= tiempoFrame4)
            {
                animadorHijo.speed = 0f; //Congelamos al personaje
                //Broly se espara en ese frame el tiempo que le hayamos puesto
                yield return new WaitForSecondsRealtime(tiempoDePausaGrito);
                //Reanudamos la animacion y ponemos la pausa en true porque ya se hizo
                animadorHijo.speed = 1f;
                pausaHecha = true;
            }

            // 2 Apartado: Agitamiento de la camara el cual empieza si ya paso el grito
            if (pausaHecha && transcurrido >= tiempoParaEmpezarAgite)
            {
                float x = Random.Range(-1f, 1f) * intensidadAgite;
                float y = Random.Range(-1f, 1f) * intensidadAgite;

                camaraPrincipal.transform.localPosition = new Vector3(
                    posicionOriginalCamara.x + x,
                    posicionOriginalCamara.y + y,
                    posicionOriginalCamara.z);
            }
            else
            {
                //Si no toca agitar dejamos la camara en su sitio original
                camaraPrincipal.transform.localPosition = posicionOriginalCamara;
            }
            //Sumamos tiempo a nuestro cronometro y le damos permiso a lo siguiente de la cinematica
            transcurrido += Time.unscaledDeltaTime;
            yield return null;
        }
        //Volvemos la camara a su sitio y esperamos unos segundos
        camaraPrincipal.transform.localPosition = posicionOriginalCamara;
        yield return new WaitForSecondsRealtime(tiempoPostEspera);

        //Reanudamos todo y eliminamos a broly
        Time.timeScale = 1f;
        if (objetoAEliminar != null) Destroy(objetoAEliminar);
    }
}