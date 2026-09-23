using UnityEngine;
using Unity.Cinemachine;

public class RoomData : MonoBehaviour
{
    [Header("Configuración de esta sala")]
    public Collider2D walkableArea; 
    
    [Header("Límites de la Cámara")]
    public Collider2D cameraBounds; 

    void Start()
    {
        ClickManager manager = FindFirstObjectByType<ClickManager>();
        if (manager != null && walkableArea != null)
        {
            manager.walkableArea = this.walkableArea;
        }

        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        
        if (confiner != null && cameraBounds != null)
        {
            confiner.BoundingShape2D = cameraBounds;
            
            confiner.InvalidateBoundingShapeCache(); 
        }
    }
}