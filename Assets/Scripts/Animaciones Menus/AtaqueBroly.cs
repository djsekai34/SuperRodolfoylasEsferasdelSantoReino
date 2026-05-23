using System.Collections;
using UnityEngine;

public class AtaqueBroly : MonoBehaviour
{
    private Animator anim;
    private GameObject bolaBroly;
    private Vector3 posicionOriginalBola;
    private Transform padreOriginalBola;

    [Header("Configuración Animación")]
    public string nombreAnimacion = "AtaqueMenuPrincipalBroly";
    public string estadoIdle = "Idle";

    [Header("Ajustes del Proyectil")]
    public float velocidadIzquierda = 50f;
    public float tiempoVida = 1.8f;

    private bool disparada = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        //Buscamos la bola de broly que es su hijo
        foreach (Transform hijo in transform)
        {
            // Si encontramos a un hijo que tenga puesto el Tag "BrolyMenuPrincipal" hacemos lo que hay dentro del if
            if (hijo.CompareTag("BrolyMenuPrincipal"))
            {
                //Lo guardamos en una variable
                bolaBroly = hijo.gameObject;
                // Guardamos quien es el padre y esto sirve para que cuando disparemos sea a quien tiene que volver
                padreOriginalBola = transform;
                // Guardamos la posicion exacta en coordenadas locales para saber el punto exacto donde debera aparecer cuando se lance
                posicionOriginalBola = bolaBroly.transform.localPosition;
                break;
            }
        }
        //Si por alguna casualidad nos dejamos la bola encendida de primeras, la apagamos para que no se vea
        if (bolaBroly != null) bolaBroly.SetActive(false);

        //Si existe el animator empezamos la corrutina
        if (anim != null)
        {
            StartCoroutine(BucleAtaqueInfinito());
        }
    }

    IEnumerator BucleAtaqueInfinito()
    {
        //Decimos que el bucle es true para que sea infinito
        while (true)
        {
            // Reseteamos todo 
            disparada = false;
            anim.speed = 1;
            anim.Play(estadoIdle);

            //Si la bola de broly existe, la escondemos, despues la metemos dentro de su padre para que se mueva con el y por ultimo le hacemos tp a su mano usando una copia que guardamos en el start
            if (bolaBroly != null)
            {
                bolaBroly.SetActive(false);
                bolaBroly.transform.SetParent(padreOriginalBola);
                bolaBroly.transform.localPosition = posicionOriginalBola;
            }

            // Pausa de 1s en el idle
            yield return new WaitForSeconds(1f);

            // Hacemos la animacion de ataque
            anim.Play(nombreAnimacion);

            //Nos esperamos un frame para que Unity reconozca que hemos cambiado de animacion y calculamos cuanto dura esta misma
            yield return new WaitForEndOfFrame();
            float duracion = anim.GetCurrentAnimatorStateInfo(0).length;

            //Esperamos a que llege al final
            yield return new WaitForSeconds(duracion);

            // Congelamos ma broly en su ultima posicion
            anim.speed = 0;

            if (bolaBroly != null)
            {
                //Sacamos la bola de broly y la hacemos hijas del canvas, para que esta viaje por donde quiera sin que le afecte su padre
                bolaBroly.transform.SetParent(transform.parent);
                //La mostramos y esta delante de todas las imagenes y la activamos en el update
                bolaBroly.SetActive(true);
                bolaBroly.transform.SetAsLastSibling();
                disparada = true;

                // Nos esperamos el tiempo que hayamos puesto
                yield return new WaitForSeconds(tiempoVida);
            }
        }
    }

    void Update()
    {
        //Lo movemos si disparada es true y si bola broly existe
        if (disparada && bolaBroly != null)
        {
            bolaBroly.transform.Translate(Vector2.left * velocidadIzquierda * Time.deltaTime);
        }
    }
}