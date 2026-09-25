using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public RoomManager roomManager;
    public ItemInteractuar itemInteractive;

    float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    public Transform player;

    [Header("Área Caminable de la Sala Actual")]
    public Collider2D walkableArea;
    public InteractionMenu interactionMenu;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            RaycastHit2D[] hits = Physics2D.RaycastAll(clickPosition, Vector2.zero);
            bool interactuo = false;

            foreach (RaycastHit2D hit in hits)
            {
                Doors puertaClickeada = hit.collider.GetComponent<Doors>();
                if (puertaClickeada != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(WalkAndEnterDoor(puertaClickeada));
                    interactuo = true;
                    break;
                }

                NPCDialogo npcClickeado = hit.collider.GetComponent<NPCDialogo>();
                if (npcClickeado != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(WalkAndTalk(npcClickeado));
                    interactuo = true;
                    break;
                }

                ItemData clickedItem = hit.collider.GetComponent<ItemData>();
                if (clickedItem != null)
                {
                    // DELEGAMOS AL PUENTE
                    itemInteractive.ProcesarClicEnItem(clickedItem);
                    interactuo = true;
                    break;
                }
            }

            if (!interactuo)
            {
                if (interactionMenu != null) interactionMenu.HideMenu();
                WalkToPoint(clickPosition);
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

    private IEnumerator WalkAndTalk(NPCDialogo npc)
    {
        if (interactionMenu != null) interactionMenu.HideMenu();
        Vector2 destino = npc.goToPoint != null ? npc.goToPoint.position : npc.transform.position;
        if (walkableArea != null) destino = walkableArea.ClosestPoint(destino);

        yield return StartCoroutine(MoveToPoint(destino));

        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && npc.nodoInicial != null) dialogueManager.IniciarDialogo(npc.nodoInicial);
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