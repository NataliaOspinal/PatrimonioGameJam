using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public Transform player;
    public GameObject habitacionInicial;

    [Header("Conexiones de Sistemas")]
    public ClickManager clickManager;
    public CinemachineCamera camaraVirtual;
    public CinemachineConfiner2D confiner;

    [Header("Transición")]
    public Image pantallaNegra;
    public float tiempoFade = 0.5f;
    
    private GameObject habitacionActual;

    void Start()
    {
        

        if (pantallaNegra != null)
        {
            pantallaNegra.color = new Color(0, 0, 0, 0);
            pantallaNegra.raycastTarget = false; 
        }
        if (habitacionInicial != null)
        {
            habitacionActual = habitacionInicial;
            ConfigurarSala(habitacionActual);
        }
        else
        {
            Debug.LogError("No se ha asignado la 'Habitacion Inicial' en el RoomManager.");
        }
    }

    public void ProcesarPuerta(Doors puerta)
    {
        if (puerta.cambiaDeEscena)
        {
            StartCoroutine(RutinaCambiarEscena(puerta.nombreEscenaDestino, puerta.nombrePuntoAparicion));
        }
        else
        {
            CambiarHabitacion(puerta.habitacionDestino, puerta.puntoDeAparicion);
        }
    }
    public void CambiarHabitacion(GameObject nuevaHabitacion, Transform nuevoPuntoAparicion)
    {
       
        StartCoroutine(RutinaTransicion(nuevaHabitacion, nuevoPuntoAparicion));
    }

    private IEnumerator RutinaTransicion(GameObject nuevaHabitacion, Transform nuevoPuntoAparicion)
    {
        if (pantallaNegra != null)
        {
            pantallaNegra.raycastTarget = true; 
            pantallaNegra.DOFade(1f, tiempoFade); 
            yield return new WaitForSeconds(tiempoFade); 
        }

        if (habitacionActual != null) habitacionActual.SetActive(false);

        player.position = new Vector3(nuevoPuntoAparicion.position.x, nuevoPuntoAparicion.position.y, 0f);
        nuevaHabitacion.SetActive(true);
        habitacionActual = nuevaHabitacion;

        yield return new WaitForSeconds(0.5f);

        ConfigurarSala(nuevaHabitacion);

        if (pantallaNegra != null)
        {
            pantallaNegra.DOFade(0f, tiempoFade).OnComplete(() => {
                pantallaNegra.raycastTarget = false; 
            });
        }
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

            camaraVirtual.Lens.OrthographicSize = data.tamanoCamara;

            camaraVirtual.gameObject.SetActive(true);
        }
    }

    private IEnumerator RutinaCambiarEscena(string nombreEscena, string nombrePuntoAparicion)
    {
        if (pantallaNegra != null)
        {
            pantallaNegra.raycastTarget = true;
            pantallaNegra.DOFade(1f, tiempoFade);
            yield return new WaitForSeconds(tiempoFade);
        }

        //Carga la nueva escena
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        while (!operacion.isDone)
        {
            yield return null;
        }

        // Busca el punto exacto por su nombre único
        GameObject spawn = GameObject.Find(nombrePuntoAparicion);

        if (spawn != null)
        {
            // Mueve a Martín a esa posición
            player.position = new Vector3(spawn.transform.position.x, spawn.transform.position.y, 0f);
            GameObject nuevaHabitacion = spawn.transform.parent.gameObject;

            // Activamos la sala correcta
            nuevaHabitacion.SetActive(true);
            habitacionActual = nuevaHabitacion;

            // Llamamos a ConfigurarSala (que buscará el RoomData entre los hijos de esta sala)
            ConfigurarSala(habitacionActual);
        }
        else
        {
            Debug.LogError("No se encontró el punto de aparición: " + nombrePuntoAparicion + ". Revisa las mayúsculas.");
        }

        if (pantallaNegra != null)
        {
            pantallaNegra.DOFade(0f, tiempoFade).OnComplete(() => {
                pantallaNegra.raycastTarget = false;
            });
        }
    }
}
