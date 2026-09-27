using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class ClickManager : MonoBehaviour
{
    public RoomManager roomManager;
    public ItemInteractuar itemInteractive;

    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Player player;

    [Header("Área Caminable de la Sala Actual")]
    public Collider2D walkableArea;
    public InteractionMenu interactionMenu;
    private ItemData itemHovreadoAnterior;



    void Update()
    {

        if (walkableArea == null)
        {
            GameObject objetoArea = GameObject.Find("AreaNavegable");
            if (objetoArea != null)
            {
                walkableArea = objetoArea.GetComponent<Collider2D>();
                Debug.Log("¡Área conectada correctamente a: " + walkableArea.gameObject.name + "!");
            }
            else
            {
                Debug.LogWarning("¡Peligro: No se encontró ningún objeto llamado 'AreaNavegable'!");
            }
        }

        if (Mouse.current == null) return;

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

      
        bool menuAbierto = interactionMenu != null && interactionMenu.gameObject.activeSelf;

        if (!EventSystem.current.IsPointerOverGameObject() && !menuAbierto)
        {
            RaycastHit2D[] hitsHover = Physics2D.RaycastAll(mouseWorldPosition, Vector2.zero);
            ItemData itemHoverActual = null;
            int maxOrdenHover = -9999; 

            foreach (RaycastHit2D hit in hitsHover)
            {
                ItemData itemHover = hit.collider.GetComponent<ItemData>();
                
                if (itemHover != null && itemHover.textoHoverFlotante != null)
                {
                    SpriteRenderer sprite = hit.collider.GetComponent<SpriteRenderer>();
                    int ordenActual = sprite != null ? sprite.sortingOrder : 0;

                    if (ordenActual > maxOrdenHover)
                    {
                        maxOrdenHover = ordenActual;
                        itemHoverActual = itemHover;
                    }
                }
            }

            if (itemHoverActual != itemHovreadoAnterior)
            {
                if (itemHovreadoAnterior != null && itemHovreadoAnterior.textoHoverFlotante != null)
                {
                    itemHovreadoAnterior.textoHoverFlotante.SetActive(false);
                }

                if (itemHoverActual != null && itemHoverActual.textoHoverFlotante != null)
                {
                    itemHoverActual.textoHoverFlotante.SetActive(true);
                }

                itemHovreadoAnterior = itemHoverActual;
            }
        }
        else
        {
            if (itemHovreadoAnterior != null && itemHovreadoAnterior.textoHoverFlotante != null)
            {
                itemHovreadoAnterior.textoHoverFlotante.SetActive(false);
                itemHovreadoAnterior = null;
            }
        }


        
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            RaycastHit2D[] hitsClick = Physics2D.RaycastAll(mouseWorldPosition, Vector2.zero);
            bool interactuo = false;

            ItemData mejorItem = null;
            int maxOrdenVisibilidad = -9999; 

            foreach (RaycastHit2D hit in hitsClick)
            {
                Doors puertaClickeada = hit.collider.GetComponent<Doors>();
                if (puertaClickeada != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(WalkAndEnterDoor(puertaClickeada));
                    interactuo = true;
                    break;
                }

                ItemData clickedItem = hit.collider.GetComponent<ItemData>();
                if (clickedItem != null)
                {
                    SpriteRenderer sprite = hit.collider.GetComponent<SpriteRenderer>();
                    int ordenActual = sprite != null ? sprite.sortingOrder : 0;

                    if (ordenActual > maxOrdenVisibilidad)
                    {
                        maxOrdenVisibilidad = ordenActual;
                        mejorItem = clickedItem;
                    }
                }
            }

            if (!interactuo && mejorItem != null)
            {
                itemInteractive.ProcesarClicEnItem(mejorItem);
                interactuo = true;
            }

            if (!interactuo)
            {
                if (interactionMenu != null) interactionMenu.HideMenu();
                
                Vector2 destinoSeguro = mouseWorldPosition;
                if (walkableArea != null)
                {
                    destinoSeguro = walkableArea.ClosestPoint(mouseWorldPosition);
                }
                
                WalkToPoint(destinoSeguro);
            }
        }
    }

    private void WalkToPoint(Vector2 targetPosition)
    {
        if (walkableArea != null) targetPosition = walkableArea.ClosestPoint(targetPosition);
        StopAllCoroutines();
        StartCoroutine(MoveToPoint(targetPosition));
    }

    private IEnumerator WalkAndEnterDoor(Doors puerta)
    {
        if (interactionMenu != null) interactionMenu.HideMenu();
        Vector2 safePoint = puerta.goToPoint.position;
        if (walkableArea != null) safePoint = walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(MoveToPoint(safePoint));
        roomManager.CambiarHabitacion(puerta.habitacionDestino, puerta.puntoDeAparicion);
    }
    public IEnumerator MoveToPoint(Vector2 point)
    {
        player.FaceTarget(point);
        player.PlayWalk(true);

        Vector2 positionDifference = point - (Vector2)player.transform.position;
        while (positionDifference.magnitude > moveAccuracy)
        {
            player.transform.Translate(moveSpeed * positionDifference.normalized * Time.deltaTime);
            positionDifference = point - (Vector2)player.transform.position;
            yield return null;
        }
        player.transform.position = point;

        // Se detiene
        player.PlayWalk(false);
    }
}