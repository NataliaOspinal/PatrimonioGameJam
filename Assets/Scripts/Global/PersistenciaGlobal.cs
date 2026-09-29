using UnityEngine;

public class PersistenciaGlobal : MonoBehaviour
{
    private static PersistenciaGlobal instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}