using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeseoShenlong : MonoBehaviour
{
    [Header("Configuración General")]
    public AudioSource altavoz;

    [Header("Elementos de la Interfaz")]
    public GameObject textoPrincipal;
    public AudioClip audioTexto;

    [Header("Botones Finales")]
    public GameObject boton1;
    public GameObject boton2;
    public GameObject boton3;

    void Start()
    {
        // Ejecutamos el codigo para ocultar todo si nos lo hemos dejado encendido y comprobamos que tengamos un audiosource y si esta todo okey lanzamos la corrutina
        OcultarTodo();

        if (altavoz != null)
        {
            StartCoroutine(CinematicaShenlong());
        }
    }

    // Ocultamos todo
    void OcultarTodo()
    {
        if (textoPrincipal) textoPrincipal.SetActive(false);
        if (boton1) boton1.SetActive(false);
        if (boton2) boton2.SetActive(false);
        if (boton3) boton3.SetActive(false);
    }

    // Emepzamos la cinematica
    IEnumerator CinematicaShenlong()
    {
        yield return new WaitForSeconds(0.9f);
        if (textoPrincipal) textoPrincipal.SetActive(true);

        yield return new WaitForSeconds(0.6f);

        if (audioTexto != null)
        {
            altavoz.clip = audioTexto;
            altavoz.Play();
            yield return new WaitForSeconds(audioTexto.length);
        }

        yield return new WaitForSeconds(0.9f);

        if (boton1)
        {
            boton1.SetActive(true);
            yield return new WaitForSeconds(1.3f);
        }

        if (boton2)
        {
            boton2.SetActive(true);
            yield return new WaitForSeconds(1.3f);
        }

        if (boton3)
        {
            boton3.SetActive(true);
        }
    }
}