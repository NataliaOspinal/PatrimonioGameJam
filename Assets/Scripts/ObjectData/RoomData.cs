using UnityEngine;
using Unity.Cinemachine;

public class RoomData : MonoBehaviour
{
    [Header("Configuración de esta sala")]
    public Collider2D walkableArea; 
    
    [Header("Límites de la Cámara")]
    public Collider2D cameraBounds;

    // Config tamaño de camara, 5 es el estándar como yo
    public float tamanoCamara = 5f;
}