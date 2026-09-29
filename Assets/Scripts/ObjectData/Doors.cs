using UnityEngine;

public class Doors : MonoBehaviour
{
    public Transform goToPoint;

    // Tipo de transición: 0 = Local (Misma Escena), 1 = Exterior (Otra Escena)
    public bool cambiaDeEscena = false;

    // Misma escena
    public GameObject habitacionDestino;
    public Transform puntoDeAparicion;

    // Otra escena
    public string nombreEscenaDestino;
    public string nombrePuntoAparicion; // El nombre exacto del objeto GameObject en la otra escena
}