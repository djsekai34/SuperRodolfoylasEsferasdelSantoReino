using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurlesTransformacion : MonoBehaviour
{
    [Header("Detección")]
    public Transform puntoX;
    public float alcance = 9.5f;
    public string tagObjetivo = "Player";

    [Header("Referencias")]
    public Animator animadorHijo;
    public GameObject objetoAEliminar;
    public GameObject bolaEnergiaPrefab;
    public RawImage imagenResplandor;

    [Header("Ajustes Secuencia")]
    public float intensidadAgite = 0.25f;
    public float tiempoAnimacionTotal = 2.77f; 
    public float velocidadAnimacion = 0.75f;   
    public float velocidadBola = 15f;

    private Camera camaraPrincipal;
    private Vector3 posicionOriginalCamara;
    private bool activado = false;
    private bool haChocadoLaBola = false;
    private GameObject rodolfo;

    void Start()
    {
        // Buscamos la camara principal y lo guardamos en una 
        camaraPrincipal = Camera.main;
        // La imagen que usaremos para el resplandor existe  lo hacemos invisible
        if (imagenResplandor != null) imagenResplandor.canvasRenderer.SetAlpha(0f);
    }

    void Update()
    {
        // Si la secuencia ya esta activa o no tenemos el punto de salida pues entonces no hacemos nada
        if (activado || puntoX == null) return;

        // Definimos el origen y la direccion del rayo
        Vector2 origen = puntoX.position;
        Vector2 direccion = -puntoX.right;

        // Creamos una mascara para que el rayo ignore la capa Trasnformacion
        int mascaraIgnorar = LayerMask.GetMask("Transformacion");

        // Lanzamos el raycast con su origen direccion, alcance y filtrando por la mascara
        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, alcance, ~mascaraIgnorar);

        // Si el rayo choca con algo y ese algo tiene el tag de Rodolfo
        if (hit.collider != null && hit.collider.CompareTag(tagObjetivo))
        {
            // Guardamos el objeto con el que chocamos en una variable llamada rodolfo y emepzamos la corrutina
            rodolfo = hit.collider.gameObject;
            StartCoroutine(SecuenciaFinal());
        }
    }

    IEnumerator SecuenciaFinal()
    {
        // Marcamos que la secuencia ha empezado para no repetir esta y guardamos la camara
        activado = true;
        posicionOriginalCamara = camaraPrincipal.transform.localPosition;

        // Cogemos todos los scripts que tenga Rodolfo y lo desactivamos
        MonoBehaviour[] scripts = rodolfo.GetComponents<MonoBehaviour>();
        foreach (var s in scripts) s.enabled = false;

        if (animadorHijo != null)
        {
            // Formazmos que la animacion empiece desde el principio y le aplicamos la velocidad que le hayamos puesto
            animadorHijo.Play("TurlesAtaqueInicial", 0, 0f);
            animadorHijo.speed = velocidadAnimacion;
        }

        //Congelamos todo
        Time.timeScale = 0f;

        // Calculamos cuanto va a durar el temblor de la camara segun la velocidad de la animacion
        float tiempoAjustado = tiempoAnimacionTotal / velocidadAnimacion;
        float transcurrido = 0f;
        while (transcurrido < tiempoAjustado)
        {
            float porcentaje = transcurrido / tiempoAjustado;
            // Si la animacion va entre el 40% y el 85% de su reproduccion agitamos la camara
            if (porcentaje > 0.40f && porcentaje < 0.85f)
            {
                float x = Random.Range(-1f, 1f) * intensidadAgite;
                float y = Random.Range(-1f, 1f) * intensidadAgite;
                camaraPrincipal.transform.localPosition = new Vector3(posicionOriginalCamara.x + x, posicionOriginalCamara.y + y, posicionOriginalCamara.z);
            }
            // Si no esta entre ese intervalo la camara vuelve a su sitio
            else { camaraPrincipal.transform.localPosition = posicionOriginalCamara; }
            // Sumamos el tiempo real para que la animacion avase aunque el juego este en pausa
            transcurrido += Time.unscaledDeltaTime;
            yield return null;
        }
        // Nos aseguramos que vamos a dejar la camara en su sitio original al acabar el bucle
        camaraPrincipal.transform.localPosition = posicionOriginalCamara;

        // Si existe la bola entramos al if para lanzarla
        if (bolaEnergiaPrefab != null)
        {
            // Creamos la bola en el punto de salida y cojemos su componente
            GameObject bolaObj = Instantiate(bolaEnergiaPrefab, puntoX.position, Quaternion.identity);
            BolaTurlesAtaqueInicial scriptBola = bolaObj.GetComponent<BolaTurlesAtaqueInicial>();

            if (scriptBola != null)
            {
                // Le pasamos a la bola quien es el dueño de esta y cual es su objetivo
                scriptBola.scriptPrincipal = this;
                scriptBola.objetivo = rodolfo.transform;
            }
            // Empezamos la corrutina
            StartCoroutine(MoverBolaInmuneAPausa(bolaObj));
        }

        // Esperamos aqui a que la bola nos avise que ha chocado
        while (!haChocadoLaBola) yield return null;

        // Si la imagen del resplando existe
        if (imagenResplandor != null)
        {
            // Lo activamos
            imagenResplandor.gameObject.SetActive(true);
            imagenResplandor.canvasRenderer.SetAlpha(1f);

            // --- TRUCO CLAVE: Desactivamos el render para que "desaparezca" sin morir el script ---
            // Desactivamos el render mientras tenemos la pantalla blanca
            if (objetoAEliminar != null)
            {
                // Buscamos su sprite para ocultarlo
                SpriteRenderer sr = objetoAEliminar.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                // Buscamos su collider para desactivarlo tambien
                Collider2D col = objetoAEliminar.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }

            // Mantenemos la pantalla en blanco 3s
            yield return new WaitForSecondsRealtime(3.0f);

            // Emepzamos a quitarlo poco a poco durante 1s
            imagenResplandor.CrossFadeAlpha(0f, 1f, true);
            yield return new WaitForSecondsRealtime(1f);
        }

        // Devolvemos todo a la normalidad y liberamos a rodolfo
        Time.timeScale = 1f;
        if (animadorHijo != null) animadorHijo.speed = 1f;
        foreach (var s in scripts) s.enabled = true; 

        // Al terminar la corrutina eliminamos a turles
        if (objetoAEliminar != null) Destroy(objetoAEliminar);
    }

    // Metodo que nos avisara para ver si la bola se ha chocado para empezar el resplandor
    public void LanzarResplandorFinal() { haChocadoLaBola = true; }

    // Movemos la bola aunque tengamos todo parado
    IEnumerator MoverBolaInmuneAPausa(GameObject bola)
    {
        while (bola != null)
        {
            bola.transform.Translate(Vector3.left * velocidadBola * Time.unscaledDeltaTime);
            yield return null;
        }
    }
}