using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Transform player;

    // Límites de movimiento del jugador
    public float minY = -4.0f;
    public float maxY = -1.0f;
    public float minX = -8.0f;
    public float maxX = 8.0f;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.GetComponent<ItemData>() != null)
            {
                ItemData clickedItem = hit.collider.GetComponent<ItemData>();
                InteractWithItem(clickedItem);
            }
            else
            {
                WalkToPoint(clickPosition);
            }
        }
    }

    // Camina libremente respetando los límites
    private void WalkToPoint(Vector2 targetPosition)
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        StopAllCoroutines();
        StartCoroutine(MoveToPoint(targetPosition));
    }

    // Inicia el movimiento hacia el punto de destino del ítem
    private void InteractWithItem(ItemData item)
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndInteract(item));
    }

    private IEnumerator MoveAndInteract(ItemData item)
    {
        // Va hacia el goToPoint del ItemData
        yield return StartCoroutine(MoveToPoint(item.goToPoint.position));

        // Próximo evento pal item o evento de interacción
        Debug.Log("Llegó al item " + item.gameObject.name);
    }

    public IEnumerator MoveToPoint(Vector2 point)
    {
        Vector2 positionDifference = point - (Vector2)player.position;
        while (positionDifference.magnitude > moveAccuracy)
        {
            player.Translate(moveSpeed * positionDifference.normalized * Time.deltaTime);
            positionDifference = point - (Vector2)player.position;
            yield return null;
        }
        player.position = point;
    }// auuuuuu
}