using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))] // Obligamos a que tenga un lineRenderer si no lo tiene lo crea
public class FinDeNivel : MonoBehaviour
{
    [Header("Ajustes del Nivel")]
    [SerializeField] private string nombreSiguienteNivel = "Martos";
    [SerializeField] private float tiempoParaCargar = 5f;
    [SerializeField] private int bolasNecesarias = 2; 

    public static bool rodolfoEsInmortal = false;

    private float cronometro = 0f;
    private bool rodolfoEstaEncima = false;
    private bool cargando = false;

    [Header("Configuración del Neón")]
    [SerializeField] private Color colorNeon = Color.red;
    [SerializeField] private float grosorNeon = 0.02f;      
    [SerializeField] private float tiempoEncendido = 0.5f;  
    [SerializeField] private float tiempoApagado = 0.5f;

    private LineRenderer lineRenderer;
    private BoxCollider2D miCollider;
    private float cronometroNeon = 0f; 
    private Texture2D fondoNegroTextura;


     void Start()
    {
        // Obtenemos el lineRenderer y el boxcollider al iniciar
        lineRenderer = GetComponent<LineRenderer>();
        miCollider = GetComponent<BoxCollider2D>();

        // Si el collider y el line renderer existen llamamos la funcion
        if (miCollider != null && lineRenderer != null)
        {
            ControlarNeon();
        }

        // Creamos una textura negra para el fonod del OnGui 
        fondoNegroTextura = new Texture2D(1, 1);
        fondoNegroTextura.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.8f)); 
        fondoNegroTextura.Apply();

    }

    void Update()
    {
        // Aplicaremos el efecto de parpadeo de forma continua
        if (lineRenderer != null)
        {           
                // Sumamamos el tiempo real que pasa entre frames a nuestro contador
                cronometroNeon += Time.deltaTime;

                // Calculamos el tiempo total del ciclo entre encenderse y apagarse
                float tiempoCicloTotal = tiempoEncendido + tiempoApagado;

                // Usanbdo el % el tiempo se resetea cada vez que se haya echo un ciclo
                float tiempoActualEnCiclo = cronometroNeon % tiempoCicloTotal;

                // Si el tiempo esta entre el rango que se puede encender lo mostramos si no se apaga
                if (tiempoActualEnCiclo < tiempoEncendido)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.startColor = colorNeon;
                    lineRenderer.endColor = colorNeon;
                }
                else
                {
                    lineRenderer.enabled = false;
                }
        }

        // Si Rodolfo esta encima y tiene las bolas magicas necesarias y no ha empezado a cargar el nivel
        if (rodolfoEstaEncima && GameManager.puntosTotales >= bolasNecesarias && !cargando)
        {
            // Cada actualizacion de frame vamos sumando tiempo
            cronometro += Time.deltaTime;

            //Si el cronometro llega al tiempo defenido anteriormente carga la siguiente escena
            if (cronometro >= tiempoParaCargar)
            {
                CargarSiguienteEscena();
            }
        }
        //Si nos salimos se resetea el tiempo
        else if (!rodolfoEstaEncima)
        {
            cronometro = 0f;
        }
    }

    //Verificamos si ha entrado rodolfo si esta dentro activiamos los 5s y lo hacemos inmortal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = true;
            rodolfoEsInmortal = true;
        }
    }

    //Aqui hacemos lo contrario le quitamos la inmortalidad y resteamos la cuenta
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rodolfoEstaEncima = false;
            rodolfoEsInmortal = false;
            cargando = false;
        }
    }

    void CargarSiguienteEscena()
    {
        cargando = true; //Condicion de seguridad para que solo se ejecute otra vez
        rodolfoEsInmortal = false; // Le quitamos la inmortalidad
        GameManager.GuardarProgresoNivel(); // Llamamos al gamemanager para guardar bien las bolas cogidas
        SceneManager.LoadScene(nombreSiguienteNivel);
    }


    // Usamos un metodo auxiliar para configurar el lineRenderer y mapear el contorno de nuestro boxcollider
    void ControlarNeon()
    {
        // Buscamos el shader por defecto de unity y creamos un material limpio, si no hacemos esto saldra rosita
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Activamos el bucle para que unity cierra el cuadrado de forma correcta
        lineRenderer.loop = true;

        // Marcamos que el grosor sea constante
        lineRenderer.startWidth = grosorNeon;
        lineRenderer.endWidth = grosorNeon;

        // Le decimos a nuestro componente que guarde el espacio para 4 puntos, que en concreto seran las 4 esquinas
        lineRenderer.positionCount = 4;

        // Almacenamos de forma local el ancho y alto del cuadrado mas su descentradro
        Vector2 tamaño = miCollider.size;
        Vector2 centro = miCollider.offset;

        // Calculamos la posicion de cada esquina de nuestro cuadrado sumando/restando la mitad del ancho (tamaño.x / 2) y la mitad del alto (tamaño.y / 2) desde el centro.
        // Usamos transform.TransformPoint para que este pase las coordenadas de modo local a coordenadas reales del mundo de unity de forma corretca, por si lo editamos el collider en un futuro este se seguira viendo canela
        Vector3 esquinaTopLeft = transform.TransformPoint(new Vector3(centro.x - tamaño.x / 2, centro.y + tamaño.y / 2, 0));
        Vector3 esquinaTopRight = transform.TransformPoint(new Vector3(centro.x + tamaño.x / 2, centro.y + tamaño.y / 2, 0));
        Vector3 esquinaBotRight = transform.TransformPoint(new Vector3(centro.x + tamaño.x / 2, centro.y - tamaño.y / 2, 0));
        Vector3 esquinaBotLeft = transform.TransformPoint(new Vector3(centro.x - tamaño.x / 2, centro.y - tamaño.y / 2, 0));

        // Dibujamos las lineas
        lineRenderer.SetPosition(0, esquinaTopLeft);
        lineRenderer.SetPosition(1, esquinaTopRight);
        lineRenderer.SetPosition(2, esquinaBotRight);
        lineRenderer.SetPosition(3, esquinaBotLeft);
    }

    // Creamos una funcion OnGUI que dibuja lo que deseemos en la pantalla sin tener que crear un canvas
    void OnGUI()
    {
        // Si Rodolfo esta encima, tiene las bolas magicas y no ha empezado la carga de la siguiente escena
        if (rodolfoEstaEncima && GameManager.puntosTotales >= bolasNecesarias && !cargando)
        {
            // Creamos un contenedor para mostrar el mensaje, con el fondo que hemos creado en el start
            GUIStyle estiloTexto = new GUIStyle();
            estiloTexto.fontSize = 24;
            estiloTexto.fontStyle = FontStyle.Bold;
            estiloTexto.alignment = TextAnchor.MiddleCenter;
            estiloTexto.normal.textColor = Color.white; // Letras blancas fijas
            estiloTexto.normal.background = fondoNegroTextura; // Fondo negro aplicado

            // Calculamos cuanto tiempo falta para poder cargar el siguiente nivel usamos Mathf.Max para que nunca haya negativos
            float tiempoRestante = Mathf.Max(0, tiempoParaCargar - cronometro);

            // Constuimos el menesaje que este dependera de que nivel estemos
            string mensaje = "";

            // Si estamos en Alcala usaremos un mensaje y si estamos en el resto de niveles otro,  usamos F1 para que solo haya un decimal
            if (SceneManager.GetActiveScene().name == "AlcalaLaReal")
            {
                // Mensaje místico especial para este nivel concreto
                mensaje = "Podrás pedir tu deseo en: " + tiempoRestante.ToString("F1") + "s ";
            }
            else
            {
                // Construimos el mensaje original para el resto de niveles usando F1 para que solo haya un decimal
                mensaje = "Cargando siguiente nivel en: " + tiempoRestante.ToString("F1") + "s ";
            }

            // Creamos un rectangulo virtual que gracias a la funcion Rect esta calculara dinamicamente la pantalla para que se vea siempre donde deseemos
            Rect posicionCaja = new Rect(Screen.width / 2 - 275, Screen.height - 130, 550, 75);

            // Dibujamos el mensaje
            GUI.Label(posicionCaja, mensaje, estiloTexto);
        }
    }
}