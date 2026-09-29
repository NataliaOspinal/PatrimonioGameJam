using UnityEngine;

public class VistaCartaUI : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData cartaEnEscena; // Objeto en escena

    // Al hacer click en la vista de carta
    public void CerrarYGuardarCarta()
    {
        if (inventoryManager != null && cartaEnEscena != null)
        {
            // Se agrega al inventario y se desaparece de escena
            if (inventoryManager.AgregarItem(cartaEnEscena))
            {
                cartaEnEscena.gameObject.SetActive(false);
            }
        }

        // Apagamos la UI de la carta
        gameObject.SetActive(false);
    }
}