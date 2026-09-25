using UnityEngine;

public enum CategoriaInteraccion
{
    SoloVer,             // 1 - Lugares: Solo ver
    ObjetoInteractuable, // 2 - Objetos: Ver, Tocar/Llevar, Hablar
    NPC                  // 3 - NPC: Ver, Tocar, Hablar
}

public class ItemData : MonoBehaviour
{
    // Configuración de Interacción
    public CategoriaInteraccion categoria = CategoriaInteraccion.ObjetoInteractuable;
    public Transform goToPoint;

    // Identificador único del objeto
    public string idItem;
    public Sprite iconoInventario;

    // Textos del inventario
    public string nombreObjeto;
    [TextArea(2, 4)] // multilinea
    public string descripcionObjeto;
}
