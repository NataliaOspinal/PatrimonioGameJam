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
            // para mostrar el menú radial, se pasa el item y la posición del mouse
            interactionMenu.ShowMenu(item);
        }
    }

    // Click en el menú radial
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

        // Lee data del item y ejecuta la acción correspondiente
        switch (accion)
        {
            case "Ver":
                Debug.Log($"El jugador está viendo {item.categoria}: {item.gameObject.name}");
                break;

            case "Tocar":
                if (item.categoria == CategoriaInteraccion.SoloVer) 
                {
                    Debug.LogWarning("Este objeto es de SoloVer y no se puede tocar.");
                }
                else
                {
                    // Cambiamos item.iconoInventario por simplemente "item"
                    if (inventoryManager != null && inventoryManager.AgregarItem(item))
                    {
                        Debug.Log($"El objeto '{item.gameObject.name}' está en el inventario.");
                        item.gameObject.SetActive(false);
                    }
                }
                break;

            case "Hablar":
                if (item.categoria == CategoriaInteraccion.NPC)
                    Debug.Log("Iniciando sistema de diálogo NPC con: " + item.gameObject.name); // para incluir diálogo dsps
                else
                    Debug.Log("El jugador le está hablando a un objeto: " + item.gameObject.name);
                break;
        }
    }
}