using UnityEngine;
using Unity.Cinemachine;

public class RoomManager : MonoBehaviour
{
    public Transform player;
    public GameObject habitacionInicial;

    [Header("Conexiones de Sistemas")]
    public ClickManager clickManager;
    public CinemachineCamera camaraVirtual;
    public CinemachineConfiner2D confiner;
    
    private GameObject habitacionActual;

    void Start()
    {
        if (habitacionInicial != null)
        {
            habitacionActual = habitacionInicial;
            ConfigurarSala(habitacionActual);
        }
        else
        {
            Debug.LogError("o se ha asignado la 'Habitacion Inicial' en el RoomManager.");
        }
    }

    public void CambiarHabitacion(GameObject nuevaHabitacion, Transform nuevoPuntoAparicion)
    {
        if (habitacionActual != null)
        {
            habitacionActual.SetActive(false);
        }

        player.position = nuevoPuntoAparicion.position;

        nuevaHabitacion.SetActive(true);
        habitacionActual = nuevaHabitacion;

        ConfigurarSala(nuevaHabitacion);
    }

    private void ConfigurarSala(GameObject sala)
    {
        RoomData data = sala.GetComponentInChildren<RoomData>();
        if (data != null)
        {
            if (clickManager != null) clickManager.walkableArea = data.walkableArea;

            if (confiner != null && data.cameraBounds != null)
            {
                confiner.BoundingShape2D = data.cameraBounds;
                confiner.InvalidateBoundingShapeCache();
            }
        }
        else
        {
            Debug.LogError("ERROR: La sala " + sala.name + " no tiene el script RoomData.");
        }

        if (camaraVirtual != null)
        {
            camaraVirtual.gameObject.SetActive(false);
            
            camaraVirtual.transform.position = new Vector3(player.position.x, player.position.y, camaraVirtual.transform.position.z);
            
            camaraVirtual.gameObject.SetActive(true);
        }
    }
}
