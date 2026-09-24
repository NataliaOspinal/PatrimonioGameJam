using UnityEngine;

public class ItemData : MonoBehaviour
{
    public Transform goToPoint;

    [Header("Opciones Disponibles en el Menú")]
    public bool puedeVer = true;
    public bool puedeTocar = false;
    public bool puedeHablar = false;
    public bool puedeEntrarSalir = false;
}
