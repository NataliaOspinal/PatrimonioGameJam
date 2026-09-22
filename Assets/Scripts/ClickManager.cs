using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Transform player;

    [Header("Área Caminable de la Sala Actual")]
    public Collider2D walkableArea;

    [Header("Interfaz UI")]
    public InteractionMenu interactionMenu;

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
                interactionMenu.ShowMenu(clickedItem, screenPosition);
            }
            else
            {
                if (interactionMenu != null) interactionMenu.HideMenu();
                WalkToPoint(clickPosition);
            }
        }
    }

    private void WalkToPoint(Vector2 targetPosition)
    {
        if (walkableArea != null)
        {
            targetPosition = walkableArea.ClosestPoint(targetPosition);
        }

        StopAllCoroutines();
        StartCoroutine(MoveToPoint(targetPosition));
    }

    public void InteractWithItem(ItemData item, string accion)
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndInteract(item, accion));
    }

    private IEnumerator MoveAndInteract(ItemData item, string accion)
    {
        Vector2 safePoint = item.goToPoint.position;
        if (walkableArea != null) safePoint = walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(MoveToPoint(safePoint));

        if (accion == "Tocar")
        {
            Debug.Log("El jugador está manoseando: " + item.gameObject.name);
            // próximamente solo en cines el inventario de objetos
        }
        else if (accion == "Hablar")
        {
            Debug.Log("El jugador va a hablarle a: " + item.gameObject.name);
            // próximamente sistema de diálogo
        }
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
    }
}