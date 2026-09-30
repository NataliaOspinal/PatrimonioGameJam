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
    public ObjectiveManager objectiveManager;
    public InteriorBaul pantallaBaul;
    public GameObject panelCartaVista;
    public UnityEngine.UI.Image pantallaNegraFinal;
    public float tiempoFadeFinal = 2f;

    public ItemData prefabHelado;
    private bool yaRecibioHelado = false;

    [Header("Conexión con Cinemáticas")]
    public GameObject cinematica2;

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
                if (item.idItem == "Baul")
                {
                    if (pantallaBaul != null) pantallaBaul.IniciarExploracion();
                    break; // Cortamos aquí para que no siga con el código de abajo
                }
                if (item.idItem == "Carta")
                {
                    if (panelCartaVista != null) panelCartaVista.SetActive(true);
                    break;
                }
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
                if (item.categoria == CategoriaInteraccion.SoloVer)
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
                if (item.idItem == "Heladero" && !yaRecibioHelado)
                {
                    StartCoroutine(RutinaHablarHeladero(item));
                    break;
                }
                if (item.idItem == "Tomas")
                {
                    StartCoroutine(RutinaHablarTomas(item));
                    break;
                }
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

    private IEnumerator RutinaHablarHeladero(ItemData npcHeladero)
    {
        if (npcHeladero.nodoDialogoHablar != null)
        {
            dialogueManager.IniciarDialogo(npcHeladero.nodoDialogoHablar);
        }

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => clickManager.enabled == true);

        // Entregamos el helado
        if (inventoryManager != null && prefabHelado != null)
        {
            inventoryManager.AgregarItem(prefabHelado);
        }

        // Actualizamos la misión
        if (objectiveManager != null)
        {
            objectiveManager.CambiarObjetivoSecuencial("Habla con Don Tomás Valverde.");
        }
        yaRecibioHelado = true;
    }

    private IEnumerator RutinaUsarItem(InventorySlot slotUsado, ItemData itemDestino)
    {
        if (inventoryManager != null)
        {
            inventoryManager.ToggleInventario();
        }

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
                    dialogueManager.IniciarDialogoSimple("¡Perfecto! Ya guardé los documentos, el libro y el abrigo. Estoy listo para irme.", PersonajeHablando.Martin);
                    if (objectiveManager != null) objectiveManager.CompletarObjetivo();

                    if (cinematica2 != null)
                    {
                        cinematica2.SetActive(true);
                    }
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
    private IEnumerator RutinaHablarTomas(ItemData npcTomas)
    {
        if (npcTomas.nodoDialogoHablar != null)
        {
            dialogueManager.IniciarDialogo(npcTomas.nodoDialogoHablar);
        }

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => clickManager.enabled == true);

        clickManager.enabled = false;

        if (pantallaNegraFinal != null)
        {
            pantallaNegraFinal.gameObject.SetActive(true);
            pantallaNegraFinal.raycastTarget = true;

            float tiempo = 0;
            Color colorInicial = pantallaNegraFinal.color;
            Color colorFinal = new Color(0, 0, 0, 1); // Negro sólido

            while (tiempo < tiempoFadeFinal)
            {
                tiempo += Time.deltaTime;
                pantallaNegraFinal.color = Color.Lerp(colorInicial, colorFinal, tiempo / tiempoFadeFinal);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("99_Credits");
    }
}