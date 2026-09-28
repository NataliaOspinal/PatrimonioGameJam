using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemGuardado;
    private Image miImagen;
    private InventoryManager manager;

    void Awake()
    {
        miImagen = GetComponent<Image>();
        manager = FindFirstObjectByType<InventoryManager>();
    }

    public void SetupSlot(ItemData nuevoItem)
    {
        itemGuardado = nuevoItem;
        miImagen.sprite = nuevoItem.iconoInventario;
        miImagen.enabled = true;
    }

    public void VaciarSlot()
    {
        itemGuardado = null;
        miImagen.sprite = null;
        miImagen.enabled = false;
    }

    // Mostrar Tooltip al pasar el ratón por encima del slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemGuardado != null)
        {
            manager.MostrarTooltip(itemGuardado, eventData.position);
        }
    }

    // Ocultar Tooltip al salir del slot
    public void OnPointerExit(PointerEventData eventData)
    {
        manager.OcultarTooltip();
    }

    // Arrastrar el item del slot
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;

        manager.OcultarTooltip(); // Oculta el papel

        miImagen.color = new Color(1, 1, 1, 0.5f);
        manager.IniciarArrastre(itemGuardado.iconoInventario);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;
        manager.ActualizarPosicionArrastre(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;

        miImagen.color = Color.white;
        manager.FinalizarArrastre();

        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(eventData.position);

        RaycastHit2D[] hits = Physics2D.RaycastAll(clickPosition, Vector2.zero);

        ItemData mejorItemDestino = null;
        int maxOrdenVisibilidad = -9999;

        foreach (RaycastHit2D hit in hits)
        {
            ItemData itemDestino = hit.collider.GetComponent<ItemData>();
            if (itemDestino != null)
            {
                // Obtenemos el Sorting Order de su SpriteRenderer
                SpriteRenderer sprite = hit.collider.GetComponent<SpriteRenderer>();
                int ordenActual = sprite != null ? sprite.sortingOrder : 0;

                // Si este objeto está más adelante que el anterior que encontramos, lo guardamos como el mejor candidato
                if (ordenActual > maxOrdenVisibilidad)
                {
                    maxOrdenVisibilidad = ordenActual;
                    mejorItemDestino = itemDestino;
                }
            }
        }

        if (mejorItemDestino != null)
        {
            manager.itemInteractive.UsarItemConItem(this, mejorItemDestino);
            return;
        }

        Debug.Log("El objeto regresó al inventario.");
    }
}