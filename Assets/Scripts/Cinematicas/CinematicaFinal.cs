using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicaFinal : MonoBehaviour
{
    [Header("Configuración General")]
    public AudioSource altavoz;
    public GameObject botonSaltarCinemantica;

    [Header("Fase 1")]
    public GameObject texto1;
    public AudioClip audio1;
    public List<GameObject> objetosFase1;

    [Header("Fase 2")]
    public GameObject texto2;
    public AudioClip audio2;
    public GameObject objetoFase2;

    [Header("Fase 3")]
    public GameObject texto3;
    public AudioClip audio3;

    [Header("Fase Final")]
    public GameObject botonFinal;

    // Guardamos la referencia de la corrutina para poder pararla si saltamos
    private Coroutine miCorrutina;

    // De primeras llamamos a la funcion ocultar todo y si tenemos puesto el atltavoz emepzamos la corrutina
    void Start()
    {
        OcultarTodo();

        // Comprobamos si venimos de darle click al boton al boton para saltar la cinematica
        if (PlayerPrefs.GetInt("SaltarCinematicaActivo", 0) == 1)
        {
            // Limpiamos a 0 para que no se quede saltada para siempre y lo guardamos
            PlayerPrefs.SetInt("SaltarCinematicaActivo", 0);
            PlayerPrefs.Save();

            // Mostramos todo y no reproducimos ningun audio (Menos el del gamemanager)
            MostrarTodoDeGolpe();
            return;
        }

        // Comprobamos si el jugador ya se ha pasado el modo historia
        if (PlayerPrefs.GetInt("ModoHistoriaCompletado", 0) == 1)
        {
            // Si existe el boton y esta asigando
            if (botonSaltarCinemantica)
            {
                // Lo mostramos y lanzamos la corrutina
                botonSaltarCinemantica.SetActive(true);
                StartCoroutine(OcultarBotonSaltarTrasTiempo(5f));
            }
        }
        // Si no se ha pasado el modo historia no lo mostramos
        else
        {
            if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
        }

        if (altavoz != null) miCorrutina = StartCoroutine(SecuenciaCinematica());
    }

    // Ocultamos todo lo de la escena por si antes lo hubieras dejado todo mostrado
    void OcultarTodo()
    {
        if (texto1) texto1.SetActive(false);
        if (texto2) texto2.SetActive(false);
        if (texto3) texto3.SetActive(false);

        foreach (var obj in objetosFase1) if (obj) obj.SetActive(false);
        if (objetoFase2) objetoFase2.SetActive(false);

        if (botonFinal) botonFinal.SetActive(false);
    }

    // Ejecutamos la cinematica
    IEnumerator SecuenciaCinematica()
    {
        yield return new WaitForSeconds(1.1f);
        if (texto1) texto1.SetActive(true);

        yield return new WaitForSeconds(0.6f);
        if (audio1) { ReproducirSonido(audio1); yield return new WaitForSeconds(audio1.length); }

        yield return new WaitForSeconds(0.9f);
        foreach (var obj in objetosFase1) if (obj) obj.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (texto2) texto2.SetActive(true);

        yield return new WaitForSeconds(0.6f);
        if (audio2) { ReproducirSonido(audio2); yield return new WaitForSeconds(audio2.length); }

        yield return new WaitForSeconds(0.9f);
        if (objetoFase2) objetoFase2.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (texto3) texto3.SetActive(true);

        yield return new WaitForSeconds(0.6f);
        if (audio3) { ReproducirSonido(audio3); yield return new WaitForSeconds(audio3.length); }

        yield return new WaitForSeconds(0.9f);
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
        if (objetoFase2) objetoFase2.SetActive(true);
        if (botonFinal) botonFinal.SetActive(true);

        foreach (var obj in objetosFase1) if (obj) obj.SetActive(true);

        if (botonSaltarCinemantica) botonSaltarCinemantica.SetActive(false);
    }

}