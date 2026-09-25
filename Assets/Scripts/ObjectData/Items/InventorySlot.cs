using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    // Cuando empieza el drag (queen)
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;

        // Icono del objeto semitransparente mientras se arrastra
        miImagen.color = new Color(1, 1, 1, 0.5f);
        manager.IniciarArrastre(itemGuardado.iconoInventario);
    }

    // Mientras mueve el mouse con el objeto arrastrado
    public void OnDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;
        manager.ActualizarPosicionArrastre(eventData.position);
    }

    // Cuando suelta el clic
    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemGuardado == null) return;

        miImagen.color = Color.white;
        manager.FinalizarArrastre();

        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Lógica para usar un objeto con otro
            Debug.Log($"Intentaste usar '{itemGuardado.gameObject.name}' sobre '{hit.collider.gameObject.name}'");
        }
        else
        {
            // El objeto regresa automáticamente
            Debug.Log("El objeto regresó al inventario.");
        }
    }
}