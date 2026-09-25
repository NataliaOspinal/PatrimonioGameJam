using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class RadialMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Elementos del botón
    public GameObject textoTMP;
    public GameObject punteroObj;
    public Image marcoUI;
    public Image iconoUI;

    // Ref al menú padre
    public InteractionMenu parentMenu;

    // Para animar el puntero
    public float velocidadTransicion = 15f; // Rapidez del viaje del puntero
    private Coroutine transicionCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textoTMP != null) textoTMP.SetActive(true);

        if (punteroObj != null)
        {
            punteroObj.SetActive(true);

            // Detenemos cualquier animación previa para evitar saltos
            if (transicionCoroutine != null) StopCoroutine(transicionCoroutine);

            // Iniciamos el viaje del puntero
            transicionCoroutine = StartCoroutine(AnimarPuntero());
        }

        if (parentMenu != null) parentMenu.HighlightButton(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textoTMP != null) textoTMP.SetActive(false);

        if (punteroObj != null)
        {
            punteroObj.SetActive(false);
            if (transicionCoroutine != null) StopCoroutine(transicionCoroutine);
        }

        if (parentMenu != null) parentMenu.ResetButtons();
    }

    private IEnumerator AnimarPuntero()
    {
        Vector3 posicionInicio = transform.parent.position;
        Vector3 posicionDestino = transform.position;

        // Colocamos el puntero en el centro
        punteroObj.transform.position = posicionInicio;

        while (Vector3.Distance(punteroObj.transform.position, posicionDestino) > 0.01f)
        {
            punteroObj.transform.position = Vector3.Lerp(punteroObj.transform.position, posicionDestino, Time.deltaTime * velocidadTransicion);
            yield return null;
        }

        punteroObj.transform.position = posicionDestino;
    }

    public void CambiarColorBase(Color nuevoColor)
    {
        if (marcoUI != null) marcoUI.color = nuevoColor;
        if (iconoUI != null) iconoUI.color = nuevoColor;
    }
}