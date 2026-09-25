using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //Animación del panel de inventario
    public RectTransform panelInventario; // Sprite
    public float posicionOcultoY = 300f;  // Posición Y fuera de la pantalla
    public float posicionVisibleY = -150f; // Posición Y dentro de la pantalla
    public float velocidadTransicion = 10f;

    private bool estaAbierto = false;
    private Coroutine animacionActual;

    //Casillas (Slots)
    public Image[] slots;
    private int itemsActuales = 0;

    void Start()
    {
        // Ocultar el panel al inicio y vaciar casillas
        panelInventario.anchoredPosition = new Vector2(panelInventario.anchoredPosition.x, posicionOcultoY);
        foreach (Image slot in slots)
        {
            slot.sprite = null;
            slot.enabled = false; // Se oculta si no hay ítem
        }
    }

    // Cuando se apreta la canasta
    public void ToggleInventario()
    {
        estaAbierto = !estaAbierto;
        if (animacionActual != null) StopCoroutine(animacionActual);

        float destinoY = estaAbierto ? posicionVisibleY : posicionOcultoY;
        animacionActual = StartCoroutine(AnimarPanel(destinoY));
    }

    private IEnumerator AnimarPanel(float destinoY)
    {
        Vector2 destino = new Vector2(panelInventario.anchoredPosition.x, destinoY);
        while (Vector2.Distance(panelInventario.anchoredPosition, destino) > 1f)
        {
            panelInventario.anchoredPosition = Vector2.Lerp(panelInventario.anchoredPosition, destino, Time.deltaTime * velocidadTransicion);
            yield return null;
        }
        panelInventario.anchoredPosition = destino;
    }

    // Al tocar un objeto
    public bool AgregarItem(Sprite icono)
    {
        if (itemsActuales >= slots.Length)
        {
            Debug.LogWarning("El inventario está lleno.");
            return false; // No se pudo agregar
        }

        slots[itemsActuales].sprite = icono;
        slots[itemsActuales].enabled = true;
        itemsActuales++;
        return true;
    }
}