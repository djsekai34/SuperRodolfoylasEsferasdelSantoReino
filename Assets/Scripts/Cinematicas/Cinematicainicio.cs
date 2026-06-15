using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematicainicio : MonoBehaviour
{
    [Header("Configuración General")]
    public AudioSource altavoz;
    public GameObject botonSaltarCinemantica;

    [Header("Fase 1")]
    public GameObject texto1;
    public AudioClip clip1;
    public List<GameObject> imagenes7;

    [Header("Fase 2")]
    public GameObject texto2;
    public AudioClip clip2;
    public List<GameObject> imagenes3;

    [Header("Fase 3")]
    public GameObject texto3;
    public AudioClip clip3;

    [Header("Fase Final - Rodolfo")]
    public GameObject rodolfo;
    public GameObject texto4;
    public AudioClip clip4;
    public GameObject botonFinal;

    // Guardamos la referencia de la corrutina para poder pararla si saltamos
    private Coroutine miCorrutina;

    void Start()
    {
        // Ejecutamos el codigo para ocultar todo si nos lo hemos dejado encendido y comprobamos que tengamos un audiosource y si esta todo okey lanzamos la corrutina
        OcultarTodo();


        DatosJuego datos = SaveGame.Cargar();

        // Comprobamos si venimos de darle click al botón para saltar la cinemática
        if (datos.saltarCinematicaActivo == 1)
        {
            // Limpiamos a 0 para que no salga el boton todo el rato
            datos.saltarCinematicaActivo = 0;

            // Guardamos el cambio de manera inmediata en el json
            SaveGame.Guardar(datos);

            // Mostramos todo
            MostrarTodoDeGolpe();
            return;
        }

        // Comprobamos si el jugador ya se ha pasado el modo historia
        if (datos.modoHistoriaCompletado == 1)
        {
            // Si existe el botón y está asignado
            if (botonSaltarCinemantica)
            {
                // Lo mostramos y lanzamos la corrutina para que desaparezca a los 5 segundos
                botonSaltarCinemantica.SetActive(true);
                StartCoroutine(OcultarBotonSaltarTrasTiempo(5f));
            }
        }
        // Si no se ha pasado el modo historia, nos aseguramos de que el botón esté oculto
        else
        {
            if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
        }

        if (altavoz != null) miCorrutina = StartCoroutine(CinematicaInicial());
    }

    // Ocultamos todo
    void OcultarTodo()
    {
        if (texto1) texto1.SetActive(false);
        if (texto2) texto2.SetActive(false);
        if (texto3) texto3.SetActive(false);
        if (texto4) texto4.SetActive(false);
        if (rodolfo) rodolfo.SetActive(false);
        if (botonFinal) botonFinal.SetActive(false);

        foreach (var img in imagenes7) if (img) img.SetActive(false);
        foreach (var img in imagenes3) if (img) img.SetActive(false);
    }

    // La corrutina de la cienamtica
    IEnumerator CinematicaInicial()
    {
        // Esperamos un chispo para mostrar el texto y mostramos el texto
        yield return new WaitForSeconds(0.8f);
        if (texto1) texto1.SetActive(true);

        // Esperamos unos segundos y si hay algun clip paramos todo y reproducimos el audio
        yield return new WaitForSeconds(0.5f);
        if (clip1) { ReproducirSonido(clip1); yield return new WaitForSeconds(clip1.length); }

        // Vovlemos a esperar un poco y mostramos las 7 imagenes
        yield return new WaitForSeconds(0.8f);
        foreach (var img in imagenes7) if (img) img.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (texto2) texto2.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (clip2) { ReproducirSonido(clip2); yield return new WaitForSeconds(clip2.length); }

        yield return new WaitForSeconds(0.8f);
        foreach (var img in imagenes3) if (img) img.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (texto3) texto3.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (clip3) { ReproducirSonido(clip3); yield return new WaitForSeconds(clip3.length); }

        yield return new WaitForSeconds(0.8f);

        if (rodolfo) rodolfo.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        if (texto4) texto4.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (clip4) { ReproducirSonido(clip4); yield return new WaitForSeconds(clip4.length); }

        yield return new WaitForSeconds(0.8f);
        if (botonFinal) botonFinal.SetActive(true);

        if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
    }

    // Para reproducir los audios
    void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && altavoz != null)
        {
            altavoz.clip = clip;
            altavoz.Play();
        }
    }

    // Tenemos esta corrutina auxiliar para que cuando pase los x segundos pues se oculte
    IEnumerator OcultarBotonSaltarTrasTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
    }

    // Metodo donde activamos toda la cinematica de golpe
    void MostrarTodoDeGolpe()
    {
        if (texto1) texto1.SetActive(true);
        if (texto2) texto2.SetActive(true);
        if (texto3) texto3.SetActive(true);
        if (texto4) texto4.SetActive(true);
        if (rodolfo) rodolfo.SetActive(true);
        if (botonFinal) botonFinal.SetActive(true);

        foreach (var img in imagenes7) if (img) img.SetActive(true);
        foreach (var img in imagenes3) if (img) img.SetActive(true);

        if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
    }
}