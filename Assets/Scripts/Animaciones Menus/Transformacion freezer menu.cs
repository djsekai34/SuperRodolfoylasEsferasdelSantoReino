using System.Collections;
using UnityEngine;

public class Transformacionfreezermenu : MonoBehaviour
{
    private Animator anim;
    public string nombreAnimacion = "TransformacionFreezerMenuPrincipal";
    public float pausaFinal = 1.0f; //Tiempo que vamnos a congelar a freezer en el ultimo frame

    //Obtenemos el animator y si existe lanzamos la corrutina que lo gestione
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            StartCoroutine(BucleEterno());
        }
    }
    //Corrutina para hacer la animacion y la pausamos
    IEnumerator BucleEterno()
    {
        while (true)
        {
            // Formzamos a que la animacion se haga si o si y ponemnos speed para que se mueva
            anim.Play(nombreAnimacion, 0, 0f); // el 0 central indica que capa del animator estamos usando
            anim.speed = 1;

            // Esperamos un frame para que se actualize el animator y reproducir bien la animacion
            yield return new WaitForEndOfFrame();

            // Calculamos la duración de la animacion
            float duracion = anim.GetCurrentAnimatorStateInfo(0).length;

            // Pausamos la ejecución del script el tiempo exacto que dura la animación para que Freezer termine de transformarse antes de quedarse quieto.
            yield return new WaitForSeconds(duracion);

            // Pausamos la animacion
            anim.speed = 0;

            // Esperamos la pausa que le hayamos puesto
            yield return new WaitForSeconds(pausaFinal);
        }
    }
}