using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // Panel del inventario y sus posiciones
    public RectTransform panelInventario;
    public float posicionOcultoY = 300f;
    public float posicionVisibleY = -150f;
    public float velocidadTransicion = 10f;

    private bool estaAbierto = false;
    private Coroutine animacionActual;

    // Slots del inventario y el �cono de arrastre
    public InventorySlot[] slots;
    public Image iconoArrastre;

    // Tooltip para mostrar informaci�n del item
    public GameObject panelTooltip; // Imagen
    public TextMeshProUGUI textoNombre; // El texto de t�tulo
    public TextMeshProUGUI textoDescripcion; // El texto de la descripci�n
    public Vector2 offsetTooltip = new Vector2(50f, -50f);

    // Conexi�n con itemInteractive
    public ItemInteractuar itemInteractive;

    void Start()
    {
        panelInventario.anchoredPosition = new Vector2(panelInventario.anchoredPosition.x, posicionOcultoY);
        foreach (InventorySlot slot in slots)
        {
            slot.VaciarSlot();
        }
        iconoArrastre.gameObject.SetActive(false);
        if (panelTooltip != null) panelTooltip.SetActive(false);
    }

    public void ToggleInventario()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ReproducirSFX("paper");
        }

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

    // Recibe data del item y lo agrega al primer slot vac�o de izquierda a derecha
    public bool AgregarItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // Si encuentra una casilla vac�a, pone el objeto ah� y corta la funci�n
            if (slots[i].itemGuardado == null)
            {
                slots[i].SetupSlot(item);
                return true;
            }
        }

        // Si el bucle termina y no encontr� vac�os, el inventario est� lleno
        Debug.LogWarning("El inventario est� lleno.");
        return false;
    }

    // Recoge los objetos que quedan, limpia todo, y los vuelve a colocar desde la izquierda
    public void ReorganizarInventario()
    {
        System.Collections.Generic.List<ItemData> itemsSobrevivientes = new System.Collections.Generic.List<ItemData>();

        // Guardar todos los �tems que a�n existen en la lista temporal
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemGuardado != null)
            {
                itemsSobrevivientes.Add(slot.itemGuardado);
                slot.VaciarSlot(); // Vaciamos la casilla temporalmente
            }
        }

        // Vuelve a colocarlos en orden desde el �ndice 0
        for (int i = 0; i < itemsSobrevivientes.Count; i++)
        {
            slots[i].SetupSlot(itemsSobrevivientes[i]);
        }
    }

    // Funciones para manejar el arrastre del �cono del objeto
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

    // Tooltip
    public void MostrarTooltip(ItemData item, Vector2 posicionMouse)
    {
        textoNombre.text = item.nombreObjeto;
        textoDescripcion.text = item.descripcionObjeto;

        // Posiciona el papel cerca del rat�n usando el offset para que el cursor no lo tape
        panelTooltip.transform.position = posicionMouse + offsetTooltip;
        panelTooltip.SetActive(true);
    }

    public void OcultarTooltip()
    {
        if (panelTooltip != null) panelTooltip.SetActive(false);
    }
}