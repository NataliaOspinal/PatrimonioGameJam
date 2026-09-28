using System.Collections;
using UnityEngine;

public class CinematicaSegunda : MonoBehaviour
{
    [Header("Managers")]
    public DialogueManager dialogueManager;
    public ClickManager clickManager;

    [Header("Actores")]
    public GameObject marianaNPC;
    public GameObject ayudanteNPC;
    public GameObject martinPlayer;

    [Header("Animators")]
    public Animator animMariana;
    public Animator animAyudante;
    public Animator animMartin;

    [Header("Posiciones (Waypoints)")]
    public Transform puntoPuerta;
    public Transform puntoDejarBaul;
    public Transform puntoMarianaEspera;
    public Transform puntoMartinBaul; // donde camina Martín para ver el baúl
    public Transform puntoSalida;
    public float velocidadCaminar = 3f;
    public float velocidadCorrerAyudante = 7f;

    [Header("Utilería (El Baúl)")]
    public GameObject baulEnManoAyudante;
    public GameObject baulEnSueloCerrado;
    public GameObject baulEnSueloAbierto; // cuando Martín lo levanta ligeramente

    [Header("Nodos de Diálogo")]
    public DialogueNode dialogoParte1; 
    public DialogueNode dialogoParte2; 
    public DialogueNode dialogoParte3; 

    void OnEnable()
    {
        if (clickManager == null) clickManager = FindFirstObjectByType<ClickManager>();
        if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();

        DialogueManager.AlCambiarHablante += ActualizarAnimacionesHabla;
        
        StartCoroutine(RutinaCinematica2());
    }

    void OnDisable()
    {
        DialogueManager.AlCambiarHablante -= ActualizarAnimacionesHabla;
    }

    private IEnumerator RutinaCinematica2()
    {
        clickManager.enabled = false;
        marianaNPC.SetActive(true);
        ayudanteNPC.SetActive(true);
        baulEnManoAyudante.SetActive(true);
        baulEnSueloCerrado.SetActive(false);
        if (baulEnSueloAbierto != null) baulEnSueloAbierto.SetActive(false);

        ForzarMirada(martinPlayer, false, false); 

        
        StartCoroutine(CaminarActor(marianaNPC, animMariana, puntoMarianaEspera, true)); 
        yield return StartCoroutine(CaminarActor(ayudanteNPC, animAyudante, puntoDejarBaul, true));

        baulEnManoAyudante.SetActive(false);
        baulEnSueloCerrado.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        
        yield return StartCoroutine(CaminarActor(ayudanteNPC, animAyudante, puntoSalida, true, velocidadCorrerAyudante));
        ayudanteNPC.SetActive(false);

        
        ForzarMirada(marianaNPC, true, true); 

        
        dialogueManager.IniciarDialogo(dialogoParte1);
        yield return new WaitUntil(() => clickManager.enabled == true);
        clickManager.enabled = false;

       
        yield return StartCoroutine(CaminarActor(martinPlayer, animMartin, puntoMartinBaul, false));
        ForzarMirada(martinPlayer, true, false); // Aseguramos que mire el baúl

       
        dialogueManager.IniciarDialogo(dialogoParte2);
        yield return new WaitUntil(() => clickManager.enabled == true);
        clickManager.enabled = false;

    
        baulEnSueloCerrado.SetActive(false);
        if (baulEnSueloAbierto != null) baulEnSueloAbierto.SetActive(true);
        yield return new WaitForSeconds(0.5f); // Pausa dramática al abrirlo

        dialogueManager.IniciarDialogo(dialogoParte3);
        yield return new WaitUntil(() => clickManager.enabled == true);
        
        
        PlayerPrefs.SetInt("Cinematica2Vista", 1);
        PlayerPrefs.Save();
        
        // aquí se puede escender la mision 2
        clickManager.enabled = true;
    }


    private IEnumerator CaminarActor(GameObject actor, Animator anim, Transform destino, bool spriteInvertidoBase, float velOverride = -1f)
    {
        if (anim != null) anim.SetBool("isWalking", true);

        float direccionX = destino.position.x - actor.transform.position.x;
        if (Mathf.Abs(direccionX) > 0.1f)
        {
            float multiplicador = spriteInvertidoBase ? -1f : 1f;
            float escalaX = multiplicador * Mathf.Sign(direccionX) * Mathf.Abs(actor.transform.localScale.x);
            actor.transform.localScale = new Vector3(escalaX, actor.transform.localScale.y, actor.transform.localScale.z);
        }

        float velocidadAplicada = velOverride > 0 ? velOverride : velocidadCaminar;

        while (Vector2.Distance(actor.transform.position, destino.position) > 0.05f)
        {
            Vector3 posDestinoSegura = new Vector3(destino.position.x, destino.position.y, actor.transform.position.z);
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, posDestinoSegura, velocidadAplicada * Time.deltaTime);
            yield return null;
        }

        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void ForzarMirada(GameObject personaje, bool mirarDerecha, bool spriteInvertidoBase)
    {
        Vector3 escala = personaje.transform.localScale;
        float multiplicador = spriteInvertidoBase ? -1f : 1f;
        float signoDestino = mirarDerecha ? 1f : -1f;
        
        float nuevaEscalaX = multiplicador * signoDestino * Mathf.Abs(escala.x);
        personaje.transform.localScale = new Vector3(nuevaEscalaX, escala.y, escala.z);
    }

    private void ActualizarAnimacionesHabla(PersonajeHablando hablanteActual)
    {
        if (animMartin != null) 
            animMartin.SetBool("isTalking", hablanteActual == PersonajeHablando.Martin);
            
        if (animMariana != null) 
            animMariana.SetBool("isTalking", hablanteActual == PersonajeHablando.Mariana);
    }
}
