using System.Collections;
using UnityEngine;

public class ItemInteractuar : MonoBehaviour
{
    public ClickManager clickManager;
    public InteractionMenu interactionMenu;
    public InventoryManager inventoryManager;

    // Click en el mundo
    public void ProcesarClicEnItem(ItemData item)
    {
        clickManager.StopAllCoroutines();
        StartCoroutine(WalkAndShowMenu(item));
    }

    private IEnumerator WalkAndShowMenu(ItemData item)
    {
        if (interactionMenu != null) interactionMenu.HideMenu();

        Vector2 safePoint = item.goToPoint.position;
        if (clickManager.walkableArea != null)
            safePoint = clickManager.walkableArea.ClosestPoint(safePoint);

        // Usa el sistema de movimiento del ClickManager
        yield return StartCoroutine(clickManager.MoveToPoint(safePoint));

        if (interactionMenu != null)
        {
            // para mostrar el men� radial, se pasa el item y la posici�n del mouse
            interactionMenu.ShowMenu(item);
        }
    }

    // Click en el men� radial
    public void EjecutarAccion(ItemData item, string accion)
    {
        clickManager.StopAllCoroutines();
        StartCoroutine(MoveAndExecute(item, accion));
    }

    private IEnumerator MoveAndExecute(ItemData item, string accion)
    {
        Vector2 safePoint = item.goToPoint.position;
        if (clickManager.walkableArea != null)
            safePoint = clickManager.walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(clickManager.MoveToPoint(safePoint));

        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();

        // Lee data del item y ejecuta la acci�n correspondiente
        switch (accion)
        {
            case "Ver":
                if (item.nodoDialogoVer != null && dialogueManager != null)
                {
                    dialogueManager.IniciarDialogo(item.nodoDialogoVer);
                }
                else
                {
                    Debug.Log($"El jugador está viendo {item.categoria}: {item.gameObject.name}");
                }
                break;

            case "Tocar":
                if (item.categoria == CategoriaInteraccion.SoloVer || item.categoria == CategoriaInteraccion.NPC) 
                {
                    Debug.LogWarning("No puedes recoger esto.");
                }
                else
                {
                    // Cambiamos item.iconoInventario por simplemente "item"
                    if (inventoryManager != null && inventoryManager.AgregarItem(item))
                    {
                        Debug.Log($"El objeto '{item.gameObject.name}' est� en el inventario.");
                        item.gameObject.SetActive(false);
                    }
                }
                break;

            case "Hablar":
                if (item.nodoDialogoHablar != null && dialogueManager != null)
                {
                    dialogueManager.IniciarDialogo(item.nodoDialogoHablar);
                }
                else
                {
                    if (item.categoria == CategoriaInteraccion.NPC)
                        Debug.Log("Este NPC no tiene diálogo de Hablar asignado.");
                    else
                        Debug.Log("El jugador le está hablando a un objeto... pero no responde.");
                }
                break;
        }
    }
}