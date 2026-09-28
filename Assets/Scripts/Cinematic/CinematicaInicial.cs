using System.Collections;
using UnityEngine;

public class CinematicaInicial : MonoBehaviour
{
    [Header("Managers")]
    public DialogueManager dialogueManager;
    public ClickManager clickManager;

    [Header("Actores y Posiciones")]
    public GameObject franciscoNPC;
    public Transform puntoPuerta;
    public Transform puntoEscritorio;
    public Transform puntoSalida;
    public float velocidadCaminar = 3f;

    [Header("Animaciones y Transform")]
    public Animator animPadre;
    public Animator animMartin;
    public Transform transformMartin;

    [Header("Utilería (Sobre)")]
    public GameObject sobreManoFrancisco;
    public GameObject sobreEnMesa;
    public GameObject sobreManoMartin;

    [Header("Guion (Nodos de Diálogo)")]
    public DialogueNode dialogoParte1; 
    public DialogueNode dialogoParte2; 
    public DialogueNode dialogoParte3;
    public DialogueNode dialogoParte4;

    
    void Start()
    {
        if (clickManager == null) clickManager = FindFirstObjectByType<ClickManager>();
        if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (PlayerPrefs.GetInt("IntroVista", 0) == 1)
        {
            // si ya se vio, ocultamos a Francisco y al sobre inicial
            franciscoNPC.SetActive(false);
            if(sobreManoFrancisco != null) sobreManoFrancisco.SetActive(false);
            this.enabled = false;
            return;
        }

        // si es la primera vez, corremos la cinematica
        StartCoroutine(RutinaCinematica());
    }

    private void OnEnable()
    {
        DialogueManager.AlCambiarHablante += ActualizarAnimacionesHabla;
    }

    private void OnDisable()
    {
        DialogueManager.AlCambiarHablante -= ActualizarAnimacionesHabla;
    }

    private void ActualizarAnimacionesHabla(PersonajeHablando hablanteActual)
    {
        if (animMartin != null) 
            animMartin.SetBool("isTalking", hablanteActual == PersonajeHablando.Martin);
            
        if (animPadre != null) 
            animPadre.SetBool("isTalking", hablanteActual == PersonajeHablando.Francisco);
    }

    private IEnumerator RutinaCinematica()
    {
        clickManager.enabled = false; // bloqueamos al jugador
        sobreManoFrancisco.SetActive(true); 
        sobreEnMesa.SetActive(false);

        Vector3 escalaOriginalMartin = transformMartin.localScale;
        transformMartin.localScale = new Vector3(-Mathf.Abs(escalaOriginalMartin.x), escalaOriginalMartin.y, escalaOriginalMartin.z);

        yield return StartCoroutine(CaminarHacia(puntoPuerta));

        dialogueManager.IniciarDialogo(dialogoParte1);
        yield return new WaitUntil(() => clickManager.enabled == true);

        clickManager.enabled = false;

        ForzarMirada(franciscoNPC, true, true);

        yield return StartCoroutine(CaminarHacia(puntoEscritorio));


        sobreManoFrancisco.SetActive(false); // desaparece de su mano
        sobreEnMesa.SetActive(true);         // aparece en la mesa
        yield return new WaitForSeconds(0.5f);

        dialogueManager.IniciarDialogo(dialogoParte2);
        yield return new WaitUntil(() => clickManager.enabled == true);
        clickManager.enabled = false;

        dialogueManager.IniciarDialogo(dialogoParte3);
        yield return new WaitUntil(() => clickManager.enabled == true);
        clickManager.enabled = false;

        ForzarMirada(franciscoNPC, true, true);
        yield return StartCoroutine(CaminarHacia(puntoPuerta));

        dialogueManager.IniciarDialogo(dialogoParte4);
        yield return new WaitUntil(() => clickManager.enabled == true);
        clickManager.enabled = false;

        ForzarMirada(franciscoNPC, false, true);
        yield return StartCoroutine(CaminarHacia(puntoSalida));
        franciscoNPC.SetActive(false);

        transformMartin.localScale = escalaOriginalMartin;

        //PlayerPrefs.SetInt("IntroVista", 1); 
       // PlayerPrefs.Save();

        clickManager.enabled = true;
    }

    private void ForzarMirada(GameObject personaje, bool mirarDerecha, bool esElPadre)
    {
        Vector3 escala = personaje.transform.localScale;
        float nuevaEscalaX;

        if (esElPadre)
        {
            nuevaEscalaX = mirarDerecha ? -Mathf.Abs(escala.x) : Mathf.Abs(escala.x);
        }
        else
        {
            nuevaEscalaX = mirarDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        }
        
        personaje.transform.localScale = new Vector3(nuevaEscalaX, escala.y, escala.z);
    }

    private IEnumerator CaminarHacia(Transform destino)
    {
        if (animPadre != null) animPadre.SetBool("isWalking", true);
        float direccionX = destino.position.x - franciscoNPC.transform.position.x;
        if (Mathf.Abs(direccionX) > 0.1f)
        {
            float escalaX = -Mathf.Sign(direccionX) * Mathf.Abs(franciscoNPC.transform.localScale.x);
            franciscoNPC.transform.localScale = new Vector3(escalaX, franciscoNPC.transform.localScale.y, franciscoNPC.transform.localScale.z);
        }
        while (Vector2.Distance(franciscoNPC.transform.position, destino.position) > 0.05f)
        {
            franciscoNPC.transform.position = Vector2.MoveTowards(franciscoNPC.transform.position, destino.position, velocidadCaminar * Time.deltaTime);
            yield return null;
        }
       

        if (animPadre != null) animPadre.SetBool("isWalking", false);
    }
}
