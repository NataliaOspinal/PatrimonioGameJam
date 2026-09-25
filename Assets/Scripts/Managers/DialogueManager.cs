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

    public void IniciarDialogo(DialogueNode nodoInicial)
    {
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

        if (nodo.opciones == null || nodo.opciones.Count == 0)
        {
            CrearBoton("Terminar conversación", null);
        }
        else
        {
            foreach (OpcionDialogo opcion in nodo.opciones)
            {
                CrearBoton(opcion.textoJugador, opcion.siguienteNodo);
            }
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
            panelDialogo.SetActive(false);
        }
        else
        {
            MostrarNodo(nodoDestino);
        }
    }
}
