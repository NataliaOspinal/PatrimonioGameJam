using UnityEngine;

public class CamaraFan : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f; // Qué tan suave sigue la cámara al jugador

    // Limites de cámara
    public float minCameraX = 0f; // Límite izquierdo
    public float maxCameraX = 20f; // Límite derecho

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        // Guardamos y, z inicial de la cámara para no alterarles
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            float clampedX = Mathf.Clamp(player.position.x, minCameraX, maxCameraX);
            Vector3 targetPosition = new Vector3(clampedX, fixedY, fixedZ);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
