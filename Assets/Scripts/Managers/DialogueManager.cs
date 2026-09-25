using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI del Diálogo")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDisplayNPC;
    public Transform contenedorOpciones; 
    public GameObject prefabBotonOpcion; 

    [Header("Clic para Avanzar (Unilateral)")]
    public Button botonPantallaCompleta; // botón invisible que atrapa los clicks
    private DialogueNode nodoPendiente;

    void Start()
    {
        
        if (panelDialogo != null) panelDialogo.SetActive(false);

        if (botonPantallaCompleta != null)
        {
            botonPantallaCompleta.onClick.AddListener(AlHacerClicEnPantalla);
        }
    }

    public void IniciarDialogo(DialogueNode nodoInicial)
    {
        ClickManager clickManager = FindFirstObjectByType<ClickManager>();
        if (clickManager != null) clickManager.enabled = false;

        panelDialogo.SetActive(true);
        MostrarNodo(nodoInicial);
    }

    private void MostrarNodo(DialogueNode nodo)
    {
        textoDisplayNPC.text = nodo.textoNPC;

        foreach (Transform child in contenedorOpciones)
        {
            Destroy(child.gameObject);
        }

        if (nodo.opciones != null && nodo.opciones.Count > 0)
        {
            botonPantallaCompleta.gameObject.SetActive(false); // Apagamos el clic de pantalla completa

            foreach (OpcionDialogo opcion in nodo.opciones)
            {
                CrearBoton(opcion.textoJugador, opcion.siguienteNodo);
            }
        }
        else
        {
            botonPantallaCompleta.gameObject.SetActive(true); 
            nodoPendiente = nodo.siguienteNodoLineal;         
        }
    }

    private void CrearBoton(string texto, DialogueNode nodoDestino)
    {
        GameObject nuevoBoton = Instantiate(prefabBotonOpcion, contenedorOpciones);
        
        nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = texto;
        
        nuevoBoton.GetComponent<Button>().onClick.AddListener(() => SeleccionarOpcion(nodoDestino));
    }

    private void SeleccionarOpcion(DialogueNode nodoDestino)
    {
        if (nodoDestino == null)
        {
           TerminarDialogo();
        }
        else
        {
            MostrarNodo(nodoDestino);
        }
    }

    private void AlHacerClicEnPantalla()
    {
        if (nodoPendiente == null) TerminarDialogo();
        else MostrarNodo(nodoPendiente);
    }

    private void TerminarDialogo()
    {
        panelDialogo.SetActive(false);

        ClickManager clickManager = FindFirstObjectByType<ClickManager>();
        if (clickManager != null) clickManager.enabled = true;
    }
}
