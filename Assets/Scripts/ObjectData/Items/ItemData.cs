using UnityEngine;

public enum CategoriaInteraccion
{
    SoloVer,             // 1 - Lugares: Solo ver
    ObjetoInteractuable, // 2 - Objetos: Ver, Tocar/Llevar, Hablar
    NPC                  // 3 - NPC: Ver, Hablar
}

public class ItemData : MonoBehaviour
{
    [Header("UI Propia del Objeto")]
    public GameObject textoHoverFlotante;

    [Header("Configuración de Interacción")]
    public CategoriaInteraccion categoria = CategoriaInteraccion.ObjetoInteractuable;
    public Transform goToPoint;

    [Header("Datos del Inventario")]
    public string idItem;
    public Sprite iconoInventario;

    // Textos del inventario
    public string nombreObjeto;
    [TextArea(2, 4)] // multilinea
    public string descripcionObjeto;

    [Header("Sistema de Diálogo")]
    public DialogueNode nodoDialogoVer;    
    public DialogueNode nodoDialogoHablar;
    public DialogueNode nodoDialogoTocar;

    void Start()
    {
        if (textoHoverFlotante != null)
        {
            textoHoverFlotante.SetActive(false);
        }
    }
}
