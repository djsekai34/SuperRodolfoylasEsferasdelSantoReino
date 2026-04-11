using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;

    [Header("Canciones")]
    public AudioClip musicaMenus;
    public AudioClip musicaNivelJaen;
    public AudioClip musicaNivelCazorla;
    public AudioClip musicaNivelMartos; 
    public AudioClip musicaNivelAndujar;
    public AudioClip musicaNivelUbeda;
    public AudioClip musicaNivelTorredonjimeno;
    public AudioClip musicaNivelAlcalaLaReal;
    public AudioClip musicaPausa;
    public AudioClip musicaMuerte;

    private float tiempoGuardadoNivel = 0f;
    // Variable nueva para recordar qué canción estaba sonando antes de la pausa
    private AudioClip clipActualNivel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); return;
        }

        //Cogemos el componete audio
        audioSource = GetComponent<AudioSource>();
        //Cada vez que carge una escena le decimos que la ejecute en el OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Para que la musica siga sonando en la escena que deseemos
        audioSource.ignoreListenerPause = true;
        //Para que el audio se sincronice bien
        audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

        // Aseguramos que el AudioSource tenga el loop activado desde el inicio para que se repita la cancion siempre
        audioSource.loop = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Dependiendo de que pantalla estemos ponemos una musica u otra
        if (scene.name == "Menu Principal" || scene.name == "Creditos" || scene.name == "SelectorNivel" || scene.name == "Controles")
        {
            tiempoGuardadoNivel = 0f; //Reseteamos el contador de la musica
            CambiarClip(musicaMenus);
        }
        else if (scene.name == "Jaen" || scene.name == "JaenSN")
        {
            clipActualNivel = musicaNivelJaen; // Guardamos que la música actual 
            CambiarClip(musicaNivelJaen);
        }
        else if (scene.name == "Cazorla" || scene.name == "CazorlaSN")
        {
            clipActualNivel = musicaNivelCazorla; 
            CambiarClip(musicaNivelCazorla);
        }
        else if (scene.name == "Martos" || scene.name == "MartosSN") 
        {
            clipActualNivel = musicaNivelMartos;
            CambiarClip(musicaNivelMartos);
        }
        else if (scene.name == "Andujar" || scene.name == "AndujarSN")
        {
            clipActualNivel = musicaNivelAndujar;
            CambiarClip(musicaNivelAndujar);
        }
        else if (scene.name == "Ubeda" || scene.name == "UbedaSN")
        {
            clipActualNivel = musicaNivelUbeda;
            CambiarClip(musicaNivelUbeda);
        }
        else if (scene.name == "Torredonjimeno" || scene.name == "TorredonjimenoSN")
        {
            clipActualNivel = musicaNivelTorredonjimeno;
            CambiarClip(musicaNivelTorredonjimeno);
        }
        else if (scene.name == "AlcalaLaReal" || scene.name == "AlcalaLaRealSN")
        {
            clipActualNivel = musicaNivelAlcalaLaReal;
            CambiarClip(musicaNivelAlcalaLaReal);
        }
        else if (scene.name == "Muerte" || scene.name == "MuerteSN")
        {
            tiempoGuardadoNivel = 0f;
            CambiarClip(musicaMuerte);
        }
    }

    // Llamamos a este metodo cuando le demos al boton escape
    public void PonerMusicaPausa()
    {
        // Guardamos el punto exacto donde este la cancion del nivel que este
        tiempoGuardadoNivel = audioSource.time;
        // Ponemos la musica de la pausa y la reproducimos
        audioSource.clip = musicaPausa;
        audioSource.loop = true; // Se reinicia la cancion al acabar esta
        audioSource.Play();
    }

    // Llamamos a este metodo cuando el jugador le vuelve a dar a escape o al boton reanudar
    public void PonerMusicaNivel()
    {
        // Volvemos a poner la musica del nivel desde el segundo que estaba guardado
        if (clipActualNivel != null)
        {
            audioSource.clip = clipActualNivel;
            audioSource.time = tiempoGuardadoNivel;
            audioSource.loop = true; 
            audioSource.Play();
        }
    }

    // Método auxiliar para cambiar de canción de forma limpia
    private void CambiarClip(AudioClip clip)
    {
        if (clip == null || audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.loop = true; 
        audioSource.Play();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}