using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public RoomManager roomManager;


    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Transform player;

    public InteractionMenu menuInteraccion;

    [Header("Área Caminable de la Sala Actual")]
    public Collider2D walkableArea;

    [Header("Interfaz UI")]
    public InteractionMenu interactionMenu;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; 
            }

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

            if (hit.collider != null)
            {
                
                Doors puertaClickeada = hit.collider.GetComponent<Doors>();
                if (puertaClickeada != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(WalkAndEnterDoor(puertaClickeada));
                    return; // Cortamos la ejecución aquí
                }

                ItemData clickedItem = hit.collider.GetComponent<ItemData>();
                if (clickedItem != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(WalkAndShowMenu(clickedItem));
                    return;
                }
            }
            
            if (interactionMenu != null) interactionMenu.HideMenu();
            WalkToPoint(clickPosition);
        }
    }

    private IEnumerator WalkAndEnterDoor(Doors puerta)
    {
        if (interactionMenu != null) interactionMenu.HideMenu();

        Vector2 safePoint = puerta.goToPoint.position;
        if (walkableArea != null) safePoint = walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(MoveToPoint(safePoint));

        roomManager.CambiarHabitacion(puerta.habitacionDestino, puerta.puntoDeAparicion);
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

    private IEnumerator WalkAndShowMenu(ItemData item)
    {
        if (interactionMenu != null) interactionMenu.HideMenu();

        Vector2 safePoint = item.goToPoint.position;
        if (walkableArea != null) safePoint = walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(MoveToPoint(safePoint));

        if (interactionMenu != null)
        {
            interactionMenu.ShowMenu(item);
        }
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
        else if (accion == "Ver")
        {
            Debug.Log("El jugador está viendo: " + item.gameObject.name);
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