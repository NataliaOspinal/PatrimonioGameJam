using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ItemInteractuar : MonoBehaviour
{
    // Sistemas y managers
    public ClickManager clickManager;
    public InteractionMenu interactionMenu;
    public InventoryManager inventoryManager;
    public DialogueManager dialogueManager;

    // Sistema de diálogo d chill
    private string[] respuestasGenericas = {
        "No creo que tenga sentido usar eso ahí.",
        "Mmm... no, eso no va a funcionar.",
        "Debería intentar otra cosa.",
        "Mejor guardo esto por ahora."
    };
    private int indiceRespuesta = 0;

    // Puzzle de la maleta aka tutorial
    private int objetosEmpacados = 0;
    // Deben coincidir exactamente con el ID ITEM chequeen bien pls (para mi yo del futuro)
    private List<string> itemsRequeridos = new List<string> { "Documentos", "Libro", "Abrigo" };

    // Click en el mundo
    public void ProcesarClicEnItem(ItemData item)
    {
        clickManager.StopAllCoroutines();

        if (clickManager.player != null) clickManager.player.PlayWalk(false);

        if (interactionMenu != null)
        {
            interactionMenu.ShowMenu(item);
        }
    }

    // Menú radial
    public void EjecutarAccion(ItemData item, string accion)
    {
        switch (accion)
        {
            case "Ver":
                // Ver no requiere moverse por obvias razones (?
                EjecutarLogicaAccion(item, accion);
                break;

            case "Tocar":
            case "Hablar":
                // Tocar y Hablar requieren moverse
                StartCoroutine(MoveAndExecute(item, accion));
                break;
        }
    }

    private IEnumerator MoveAndExecute(ItemData item, string accion)
    {
        Vector2 safePoint = item.goToPoint != null ? item.goToPoint.position : item.transform.position;
        if (clickManager.walkableArea != null)
            safePoint = clickManager.walkableArea.ClosestPoint(safePoint);

        yield return StartCoroutine(clickManager.MoveToPoint(safePoint));

        // Una vez que llega, ejecuta la acción
        EjecutarLogicaAccion(item, accion);
    }

    // Lógica de cada acción
    private void EjecutarLogicaAccion(ItemData item, string accion)
    {
        switch (accion)
        {
            case "Ver":
                // Si tiene un nodo de diálogo complejo lo usa, si no, usa la descripción simple
                if (item.nodoDialogoVer != null)
                {
                    dialogueManager.IniciarDialogo(item.nodoDialogoVer);
                }
                else if (!string.IsNullOrEmpty(item.descripcionObjeto))
                {
                    dialogueManager.IniciarDialogoSimple(item.descripcionObjeto, PersonajeHablando.Martin);
                }
                break;

            case "Tocar":
                if (item.categoria == CategoriaInteraccion.SoloVer || item.categoria == CategoriaInteraccion.NPC)
                {
                    dialogueManager.IniciarDialogoSimple("No puedo recoger esto.", PersonajeHablando.Martin);
                }
                else
                {
                    if (inventoryManager != null && inventoryManager.AgregarItem(item))
                    {
                        item.gameObject.SetActive(false);
                        dialogueManager.IniciarDialogoSimple($"He recogido: {item.nombreObjeto}", PersonajeHablando.Martin);
                    }
                }
                break;

            case "Hablar":
                if (item.nodoDialogoHablar != null)
                {
                    dialogueManager.IniciarDialogo(item.nodoDialogoHablar);
                }
                else
                {
                    if (item.categoria == CategoriaInteraccion.NPC)
                        Debug.LogWarning("Este NPC no tiene un nodo de diálogo asignado.");
                    else
                        dialogueManager.IniciarDialogoSimple("No creo que deba hablarle a las cosas...", PersonajeHablando.Martin);
                }
                break;
        }
    }

    // Arrastrar del inventario 
    public void UsarItemConItem(InventorySlot slotUsado, ItemData itemDestino)
    {
        StartCoroutine(RutinaUsarItem(slotUsado, itemDestino));
    }

    private IEnumerator RutinaUsarItem(InventorySlot slotUsado, ItemData itemDestino)
    {
        Vector2 destino = itemDestino.goToPoint != null ? itemDestino.goToPoint.position : itemDestino.transform.position;
        yield return StartCoroutine(clickManager.MoveToPoint(destino));

        ItemData itemUsado = slotUsado.itemGuardado;

        if (itemDestino.idItem == "Maleta")
        {
            if (itemsRequeridos.Contains(itemUsado.idItem))
            {
                itemsRequeridos.Remove(itemUsado.idItem);
                objetosEmpacados++;
                slotUsado.VaciarSlot();
                inventoryManager.ReorganizarInventario();

                if (objetosEmpacados == 3)
                {
                    dialogueManager.IniciarDialogoSimple("Maleta lista...", PersonajeHablando.Martin);
                }
                else
                {
                    dialogueManager.IniciarDialogoSimple($"He guardado '{itemUsado.nombreObjeto}' en la maleta. Aún me faltan cosas.", PersonajeHablando.Martin);
                }
            }
            else
            {
                dialogueManager.IniciarDialogoSimple("No necesito eso en Cádiz.", PersonajeHablando.Martin);
            }
        }
        else
        {
            dialogueManager.IniciarDialogoSimple(respuestasGenericas[indiceRespuesta], PersonajeHablando.Martin);
            indiceRespuesta++;
            if (indiceRespuesta >= respuestasGenericas.Length) indiceRespuesta = 0;
        }
    }
}