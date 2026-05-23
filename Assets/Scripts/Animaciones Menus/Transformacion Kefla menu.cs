using System.Collections;
using UnityEngine;

public class TransformacionKeflamenu : MonoBehaviour
{
    private Animator anim;
    public string nombreAnimacion = "KeflaMenuPrincipal";
    public float pausaFinal = 2.0f; 
    public float duracionTotal = 3.0f; 

    void Start()
    {
        // Buscamos el animator y lo guardamos
        anim = GetComponent<Animator>();
        // Verifiamos que exista un Animator para evitar errores
        if (anim != null)
        {
            // Configuramos el Animator par aque ignore el Time.timeScale
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

            // Le ponemos velocidad en 0 para que no se reproduzca sola 
            anim.speed = 0;

            // Llamamos a una corrutina para poner un poco de retraso y evitar el tiron de Unity
            StartCoroutine(InicioSuave());
        }
    }

    IEnumerator InicioSuave()
    {
        // Esperamos un poco de frames reales para que se estabilice el framerate y iniciamos la animacion
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(BucleEternoManual());
    }

    IEnumerator BucleEternoManual()
    {
        // Iniciamos el bucle infinitamente
        while (true)
        {
            // Reiniciamos el contador de tiempo al inicio de cada ciclo de la animacion
            float cronometro = 0;

            // Mientras hayamos reproducido menos del 97% de la animacion ...
            while (cronometro < duracionTotal * 0.97f)
            {
                // Sumamso el tiempo real desde el utlimo frame a un rango de 0 a 1, si llevamos por ejemplo 1,5s en una animacion de 3s internamente tendra un progreso de 0.5
                cronometro += Time.unscaledDeltaTime;
                float progreso = Mathf.Clamp01(cronometro / duracionTotal);

                // Movemos la animacion al punto exacto que deseemos y forzamos a que se quede alli
                anim.Play(nombreAnimacion, 0, progreso);
                anim.Update(0f);

                // Esperamos al siguiente frame para continuar el bucle
                yield return null;
            }

            // Procedemos a congelar la animacion en el frame 7 que es el que deseamos
            anim.Play(nombreAnimacion, 0, 0.99f);
            anim.Update(0f);

            // Ponemos la pausa que hayamos puesto en la variable
            yield return new WaitForSecondsRealtime(pausaFinal);

            // Reseteamos todo
            anim.Play(nombreAnimacion, 0, 0f);
            anim.Update(0f);

            // Damos una pequeña pausa de 0,5s 
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}