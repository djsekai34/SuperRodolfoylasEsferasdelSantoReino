using UnityEngine;

public class BolaTurlesAtaqueInicial : MonoBehaviour
{
    // Creamos dos variables pero le ponemos [HideInInspector] para que sean publicas pero no se muestren en el inspector de unity 
    [HideInInspector] public TurlesTransformacion scriptPrincipal;
    [HideInInspector] public Transform objetivo; // Aquí guardaremos a Rodolfo

    public float distanciaParaImpacto = 0.8f;
    private bool yaChoco = false;

    void Update()
    {
        // Si la bola ya ha chocado o no tenemos el objetivo que seguir nos salimos para no tener errores
        if (yaChoco || objetivo == null) return;

        // Comparamos la posicion de la bola con el enemigo de forma manual para no depeder de la fisicas de Unity
        float distanciaActual = Vector2.Distance(transform.position, objetivo.position);

        // Si la distancia es menor o igual a la que hemos configurado
        if (distanciaActual <= distanciaParaImpacto)
        {
            // Marcamos que ya ha chocado para que no choquemos un monton de veces
            yaChoco = true;

            // Entramos al script de Turles y hacemos que salte el resplandor
            if (scriptPrincipal != null)
            {
                scriptPrincipal.LanzarResplandorFinal();
            }

            // Destruinos la bola
            Destroy(gameObject);
        }
    }
}