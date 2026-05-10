using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject panelPausa;
    public GameObject canvasHUD;

    [Header("Configuración Cámara")]
    public Camera camaraPrincipal;

    private bool juegoPausado = false;
    private int mascaraOriginal;

    void Start()
    {
        //Buscamos la camara del juego
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }

        //Hacemos "Fotografia" de la camara
        if (camaraPrincipal != null)
        {
            mascaraOriginal = camaraPrincipal.cullingMask;
        }
    }

    void Update()
    {
        //Si le hemos dado al escape y esta pausado le damos a reanudar si no a pausar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        // Marcamos que el juego está en pausa y activamos el menu
        juegoPausado = true;
        panelPausa.SetActive(true);
        //Escondemos el hud
        if (canvasHUD != null) canvasHUD.SetActive(false);

        //Congelamos el tiempo
        Time.timeScale = 0f;

        //Si tenemos un music manager ponemos la musica de la pausa
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PonerMusicaPausa();
        }

        //En esta linea le decimos a la camara principal que SOLO renderize la capa que sea UI (sera el menu de pausa)
        if (camaraPrincipal != null)
        {
            camaraPrincipal.cullingMask = 1 << LayerMask.NameToLayer("UI");
        }
    }

    public void Reanudar()
    {
        //Volvemos al juego y actvamos el canvas
        juegoPausado = false;
        panelPausa.SetActive(false);
        if (canvasHUD != null) canvasHUD.SetActive(true);

        //Reanudamos el tiempo para que fluya normal
        Time.timeScale = 1f;

        //Le decimos al MusicManager que quite la musica de pausa y ponga la de juego
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PonerMusicaNivel();
        }

        //Devolvemos a la camara a que mire lo que habia antes
        if (camaraPrincipal != null)
        {
            camaraPrincipal.cullingMask = mascaraOriginal;
        }
    }

    //Reiniciamos el nivel donde estemos actualmente
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        GameManager.ResetearPuntosParaReiniciar();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Metodo exclusivo para el SN donde nos lleva al selector de Niveles
    public void IrAlSelector()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("SelectorNivel");
    }

    //Para ir al menu
    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Principal");
    }
}