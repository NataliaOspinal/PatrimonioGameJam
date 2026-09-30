using UnityEngine;

public class AnimadorNPC : MonoBehaviour
{
    [Header("Identidad del Personaje")]
    public PersonajeHablando miPersonaje; 

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void CambiarEstadoHablar(bool estaHablando)
    {
        if (anim != null)
        {
            anim.SetBool("isTalking", estaHablando);
        }
    }
}