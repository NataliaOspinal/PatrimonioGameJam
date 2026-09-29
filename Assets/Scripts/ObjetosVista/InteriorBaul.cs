using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteriorBaul : MonoBehaviour
{
    // Conexiones con otros componentes
    public DialogueManager dialogueManager;
    public ClickManager clickManager;
    public ObjectiveManager objectiveManager;
    public CanvasGroup panelGroup;

    // Nodos de diálogo para cada objeto
    public DialogueNode nodoObjeto1;
    public DialogueNode nodoObjeto2;
    public DialogueNode nodoObjeto3;

    // Nombres de los objetos para el diálogo simple, segunda vez que se tocan
    public string nombreObjeto1 = "Pedazo de cerámica decorada";
    public string nombreObjeto2 = "Piruro de huso";
    public string nombreObjeto3 = "Tupu metálico";

    private bool visto1, visto2, visto3;
    private bool estaCerrando = false;

    // Esta función la llamaremos desde ItemInteractuar para abrir el baúl
    public void IniciarExploracion()
    {
        gameObject.SetActive(true);
        panelGroup.alpha = 1f;
        visto1 = visto2 = visto3 = estaCerrando = false;
    }

    // Funciones para los botones de la interfaz
    public void ClickObjeto1()
    {
        if (!visto1) { dialogueManager.IniciarDialogo(nodoObjeto1); visto1 = true; }
        else { dialogueManager.IniciarDialogoSimple(nombreObjeto1, PersonajeHablando.Martin); }
        ComprobarFinal();
    }

    public void ClickObjeto2()
    {
        if (!visto2) { dialogueManager.IniciarDialogo(nodoObjeto2); visto2 = true; }
        else { dialogueManager.IniciarDialogoSimple(nombreObjeto2, PersonajeHablando.Martin); }
        ComprobarFinal();
    }

    public void ClickObjeto3()
    {
        if (!visto3) { dialogueManager.IniciarDialogo(nodoObjeto3); visto3 = true; }
        else { dialogueManager.IniciarDialogoSimple(nombreObjeto3, PersonajeHablando.Martin); }
        ComprobarFinal();
    }

    private void ComprobarFinal()
    {
        if (visto1 && visto2 && visto3 && !estaCerrando)
        {
            estaCerrando = true;
            StartCoroutine(CerrarConFadeOut());
        }
    }

    private IEnumerator CerrarConFadeOut()
    {
        // Esperamos a que el ClickManager vuelva a activarse (indica si el diálogo ha terminado)
        yield return new WaitUntil(() => clickManager != null && clickManager.enabled == true);

        // Espera antes del fade out
        yield return new WaitForSeconds(0.5f);

        float duracionFade = 1.5f;
        float tiempo = 0;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(1f, 0f, tiempo / duracionFade);
            yield return null;
        }

        gameObject.SetActive(false);

        // Actualizamos la misión al terminar el desvanecimiento
        if (objectiveManager != null)
        {
            objectiveManager.MostrarObjetivo("Examinar la carta de Isabel");
        }
    }
}