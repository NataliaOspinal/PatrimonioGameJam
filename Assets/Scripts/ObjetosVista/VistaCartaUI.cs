using UnityEngine;

public class VistaCartaUI : MonoBehaviour
{
    // Inventario
    public InventoryManager inventoryManager;
    public ItemData cartaEnEscena;

    // Eventos y diálogos
    public DialogueManager dialogueManager;
    public DialogueNode dialogoFinalCarta;
    public ObjectiveManager objectiveManager;
    public GameObject puertaSalida; // para activar la puerta dsps del´diálgoo final

    public void CerrarYGuardarCarta()
    {
        // Guarda en inventario y borrar de la mesa
        if (inventoryManager != null && cartaEnEscena != null)
        {
            if (inventoryManager.AgregarItem(cartaEnEscena))
            {
                cartaEnEscena.gameObject.SetActive(false);
            }
        }

        // último diálogo
        if (dialogueManager != null && dialogoFinalCarta != null)
        {
            dialogueManager.IniciarDialogo(dialogoFinalCarta);
        }

        // actualiza objetivos
        if (objectiveManager != null)
        {
            objectiveManager.CambiarObjetivoSecuencial("Busca a don Tomás Valverde.");
        }

        // Habilita la puerta para salir del tuto xfin
        if (puertaSalida != null)
        {
            puertaSalida.SetActive(true);
        }
        
        // Apagar pantalla
        gameObject.SetActive(false);
    }
}