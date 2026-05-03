using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicaFinalMalo : MonoBehaviour
{
    [Header("Configuración General")]
    public AudioSource altavoz;

    [Header("Fase 1: Texto y Objeto Único")]
    public GameObject texto1;
    public AudioClip audio1;
    public GameObject objetoFase1;

    [Header("Fase 2: Doble Texto")]
    public GameObject texto2;
    public AudioClip audio2;
    public GameObject texto3;
    public AudioClip audio3;

    [Header("Fase 3: Los 3 Objetos")]
    public List<GameObject> objetosFase3;

    [Header("Fase 4: Texto Final y Botón")]
    public GameObject texto4;
    public AudioClip audio4;
    public GameObject botonFinal;

    // De primeras llamamos a la funcion ocultar todo y si tenemos puesto el atltavoz emepzamos la corrutina
    void Start()
    {
        OcultarTodo();
        if (altavoz != null) StartCoroutine(SecuenciaCinematica());
    }

    // Ocultamos todo lo de la escena por si antes lo hubieras dejado todo mostrado
    void OcultarTodo()
    {
        if (texto1) texto1.SetActive(false);
        if (texto2) texto2.SetActive(false);
        if (texto3) texto3.SetActive(false);
        if (texto4) texto4.SetActive(false);

        if (objetoFase1) objetoFase1.SetActive(false);
        foreach (var obj in objetosFase3) if (obj) obj.SetActive(false);

        if (botonFinal) botonFinal.SetActive(false);
    }

    // Ejecutamos la cinematica
    IEnumerator SecuenciaCinematica()
    {
        yield return new WaitForSeconds(1.3f);
        if (texto1) texto1.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (audio1) { ReproducirSonido(audio1); yield return new WaitForSeconds(audio1.length); }

        yield return new WaitForSeconds(0.9f);
        if (objetoFase1) objetoFase1.SetActive(true);
        yield return new WaitForSeconds(1.5f); 

        if (texto2) texto2.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (audio2) { ReproducirSonido(audio2); yield return new WaitForSeconds(audio2.length); }
        yield return new WaitForSeconds(0.9f);

        if (texto3) texto3.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (audio3) { ReproducirSonido(audio3); yield return new WaitForSeconds(audio3.length); }
        yield return new WaitForSeconds(0.9f);

        foreach (var obj in objetosFase3) if (obj) obj.SetActive(true);
        yield return new WaitForSeconds(1.5f); 

        if (texto4) texto4.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        if (audio4) { ReproducirSonido(audio4); yield return new WaitForSeconds(audio4.length); }

        yield return new WaitForSeconds(0.9f);
        if (botonFinal) botonFinal.SetActive(true);
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
}
