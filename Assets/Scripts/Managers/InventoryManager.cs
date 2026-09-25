using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Panel del inventario y sus posiciones
    public RectTransform panelInventario;
    public float posicionOcultoY = 300f;
    public float posicionVisibleY = -150f;
    public float velocidadTransicion = 10f;

    private bool estaAbierto = false;
    private Coroutine animacionActual;

    // Slots del inventario y el ícono de arrastre
    public InventorySlot[] slots;
    public Image iconoArrastre;
    private int itemsActuales = 0;

    void Start()
    {
        panelInventario.anchoredPosition = new Vector2(panelInventario.anchoredPosition.x, posicionOcultoY);
        foreach (InventorySlot slot in slots)
        {
            slot.VaciarSlot();
        }
        iconoArrastre.gameObject.SetActive(false);
    }

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

    // Recibe data del item y lo agrega al inventario si hay espacio disponible
    public bool AgregarItem(ItemData item)
    {
        if (itemsActuales >= slots.Length)
        {
            Debug.LogWarning("El inventario está lleno.");
            return false;
        }

        slots[itemsActuales].SetupSlot(item);
        itemsActuales++;
        return true;
    }

    // Funciones para manejar el arrastre del ícono del objeto
    public void IniciarArrastre(Sprite sprite)
    {
        iconoArrastre.sprite = sprite;
        iconoArrastre.gameObject.SetActive(true);
    }

    public void ActualizarPosicionArrastre(Vector2 posicionPantalla)
    {
        iconoArrastre.transform.position = posicionPantalla;
    }

    public void FinalizarArrastre()
    {
        iconoArrastre.gameObject.SetActive(false);
    }
}