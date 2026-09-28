using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("Fuentes")]
    public TMP_FontAsset fuentePorDefecto;

    public static Action<PersonajeHablando> AlCambiarHablante;

    [Header("Efecto de Texto")]
    public float velocidadTexto = 0.03f; 
    private bool isTyping = false;
    private bool cancelTyping = false;
    private Coroutine corrutinaEscribir;
    private DialogueNode nodoActualActivo;

    [Header("UI del Diálogo")]
    public GameObject baseDeDialogo;
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDisplayNPC;
    public Transform contenedorOpciones; 
    public GameObject prefabBotonOpcion; 

    [Header("Clic para Avanzar (Unilateral)")]
    public Button botonPantallaCompleta; // botón invisible que atrapa los clicks
    private DialogueNode nodoPendiente;

    // Jugador
    public Player player;

    //Click manager 
    public ClickManager clickManager;

    void Start()
    {
        
        if (panelDialogo != null) panelDialogo.SetActive(false);

        if (botonPantallaCompleta != null)
        {
            botonPantallaCompleta.onClick.AddListener(ClickEnPantalla);
        }
    }

    public void IniciarDialogoSimple(string texto, PersonajeHablando hablante = PersonajeHablando.Martin)
    {
        DialogueNode nodoTemp = ScriptableObject.CreateInstance<DialogueNode>();
        nodoTemp.textoNPC = texto;
        nodoTemp.hablante = hablante;

        IniciarDialogo(nodoTemp);
    }
    public void IniciarDialogo(DialogueNode nodoInicial)
    {
        if (clickManager != null) clickManager.enabled = false;

        if (baseDeDialogo != null) baseDeDialogo.SetActive(true);
        if (panelDialogo != null) panelDialogo.SetActive(true);
        MostrarNodo(nodoInicial);
    }

    private void MostrarNodo(DialogueNode nodo)
    {
        if (nodo.fuenteEspecial != null)
        {
            textoDisplayNPC.font = nodo.fuenteEspecial;
        }
        else
        {
            if (fuentePorDefecto != null)
            {
                textoDisplayNPC.font = fuentePorDefecto;
            }
        }


        AlCambiarHablante?.Invoke(nodo.hablante);

        textoDisplayNPC.text = nodo.textoNPC;

        foreach (Transform child in contenedorOpciones)
        {
            Destroy(child.gameObject);
        }
        nodoActualActivo = nodo;

        if (corrutinaEscribir != null)
        {
            StopCoroutine(corrutinaEscribir);
        }

        corrutinaEscribir = StartCoroutine(EscribirTexto(nodo.textoNPC));
    }

    private IEnumerator EscribirTexto(string textoCompleto)
    {
        isTyping = true;
        cancelTyping = false;
        textoDisplayNPC.text = "";

        botonPantallaCompleta.gameObject.SetActive(true);

        foreach (char letra in textoCompleto.ToCharArray())
        {
            if (cancelTyping)
            {
                textoDisplayNPC.text = textoCompleto;
                break; 
            }

            textoDisplayNPC.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        isTyping = false;

        
        if (nodoActualActivo.opciones != null && nodoActualActivo.opciones.Count > 0)
        {
            botonPantallaCompleta.gameObject.SetActive(false); 
            // Recién ahora creamos los botones de respuesta
            foreach (OpcionDialogo opcion in nodoActualActivo.opciones)
            {
                CrearBoton(opcion.textoJugador, opcion.siguienteNodo);
            }
        }
        else
        {
            
            botonPantallaCompleta.gameObject.SetActive(true);
            nodoPendiente = nodoActualActivo.siguienteNodoLineal;
        }
    }

    private void CrearBoton(string texto, DialogueNode nodoDestino)
    {
        GameObject nuevoBoton = Instantiate(prefabBotonOpcion, contenedorOpciones);
        nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = texto;

        // Se inicia mini animación
        nuevoBoton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SeleccionarOpcionAnimada(nodoDestino)));
    }

    private IEnumerator SeleccionarOpcionAnimada(DialogueNode nodoDestino)
    {
        foreach (Transform child in contenedorOpciones)
        {
            Destroy(child.gameObject);
        }
    
        if (player != null) player.PlayTalk(true);

        yield return new WaitForSeconds(1.5f);

        if (player != null) player.PlayTalk(false);

        if (nodoDestino == null)
        {
            TerminarDialogo();
        }
        else
        {
            MostrarNodo(nodoDestino);
        }
    }

   public void ClickEnPantalla() 
    {
        if (isTyping)
        {
            cancelTyping = true;
        }
        else
        {
            if (nodoPendiente != null)
            {
                MostrarNodo(nodoPendiente);
            }
            else
            {
                TerminarDialogo();
            }
        }
    }

    private void TerminarDialogo()
    {
        AlCambiarHablante?.Invoke(PersonajeHablando.Ninguno);
        panelDialogo.SetActive(false);
        if (clickManager != null) clickManager.enabled = true;
    }
}
