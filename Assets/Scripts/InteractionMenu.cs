using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic; 
using DG.Tweening;

public class InteractionMenu : MonoBehaviour
{
    public ItemInteractuar itemInteractive;
    private ItemData currentItem;
    public float alturaSobreObjeto = 0.5f;

    [Header("Animación de Baraja Radial")]
    public float radioExpansion = 120f; 
    public float tiempoAnimacion = 0.4f;
    [Range(0f, 180f)]
    public float anguloArco = 120f;

    // Botones del menú radial
    public GameObject btnVer;
    public GameObject btnTocar;
    public GameObject btnHablar;
    public GameObject btnEntrarSalir;

    public RadialMenuButton[] allButtons;
    public Color normalColor = Color.white;
    public Color shadowColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    // Muestra el menú encima del objeto
    public void ShowMenu(ItemData item)
    {
        currentItem = item;
        // Por seguridad apagamos los botones
        if (btnVer != null) btnVer.SetActive(false);
        if (btnTocar != null) btnTocar.SetActive(false);
        if (btnHablar != null) btnHablar.SetActive(false);

        // Segun la categoría del objeto, activamos los botones correspondientes
        switch (item.categoria)
        {
            case CategoriaInteraccion.SoloVer:
                if (btnVer != null) btnVer.SetActive(true);
                break;

            case CategoriaInteraccion.ObjetoInteractuable:
                if (btnVer != null) btnVer.SetActive(true);
                if (btnTocar != null) btnTocar.SetActive(true);
                if (btnHablar != null) btnHablar.SetActive(true);
                break;

            case CategoriaInteraccion.NPC:
                if (btnVer != null) btnVer.SetActive(true);
               // if (btnTocar != null) btnTocar.SetActive(true);
                if (btnHablar != null) btnHablar.SetActive(true);
                break;
        }

        Vector3 nuevaPosicion = item.transform.position + new Vector3(0, alturaSobreObjeto, 0);
        nuevaPosicion.z = 0f;
        transform.position = nuevaPosicion;

        gameObject.SetActive(true);
        ResetButtons(); // Asegura que ningún botón empiece oscurecido

        AnimarAperturaRadial();
    }

    private void AnimarAperturaRadial()
    {
        List<RectTransform> botonesActivos = new List<RectTransform>();
        
        if (btnVer != null && btnVer.activeSelf) botonesActivos.Add(btnVer.GetComponent<RectTransform>());
        if (btnTocar != null && btnTocar.activeSelf) botonesActivos.Add(btnTocar.GetComponent<RectTransform>());
        if (btnHablar != null && btnHablar.activeSelf) botonesActivos.Add(btnHablar.GetComponent<RectTransform>());
        if (btnEntrarSalir != null && btnEntrarSalir.activeSelf) botonesActivos.Add(btnEntrarSalir.GetComponent<RectTransform>());

        int cantidad = botonesActivos.Count;
        if (cantidad == 0) return;

        for (int i = 0; i < cantidad; i++)
        {
            RectTransform btn = botonesActivos[i];

            btn.anchoredPosition = Vector2.zero;
            btn.localScale = Vector3.zero;

            float angulo = 90f; 

            if (cantidad > 1)
            {
                float anguloMitad = anguloArco / 2f;
                float anguloInicio = 90f + anguloMitad; 
                float pasoGrados = anguloArco / (cantidad - 1); 
                
                angulo = anguloInicio - (i * pasoGrados);
            }

            float radianes = angulo * Mathf.Deg2Rad;
            float metaX = Mathf.Cos(radianes) * radioExpansion;
            float metaY = Mathf.Sin(radianes) * radioExpansion;

            btn.DOAnchorPos(new Vector2(metaX, metaY), tiempoAnimacion).SetEase(Ease.OutBack);
            btn.DOScale(Vector3.one, tiempoAnimacion).SetEase(Ease.OutBack);
        }
    }

    public void HideMenu()
    {
        List<GameObject> todos = new List<GameObject> { btnVer, btnTocar, btnHablar, btnEntrarSalir };
        bool hayActivos = false;

        foreach (GameObject btnObj in todos)
        {
            if (btnObj != null && btnObj.activeSelf)
            {
                hayActivos = true;
                RectTransform btn = btnObj.GetComponent<RectTransform>();
                
                btn.DOAnchorPos(Vector2.zero, tiempoAnimacion / 2f).SetEase(Ease.InBack);
                btn.DOScale(Vector3.zero, tiempoAnimacion / 2f).SetEase(Ease.InBack);
            }
        }

        if (hayActivos)
        {
            DOVirtual.DelayedCall(tiempoAnimacion / 2f, () => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Hover de los botones del menú radial
    public void HighlightButton(RadialMenuButton hoveredButton)
    {
        foreach (RadialMenuButton btn in allButtons)
        {
            if (btn != null)
            {
                // Si es el botón sobre el que está el ratón, color normal. Si no, sombra.
                btn.CambiarColorBase((btn == hoveredButton) ? normalColor : shadowColor);
            }
        }
    }

    public void ResetButtons()
    {
        foreach (RadialMenuButton btn in allButtons)
        {
            if (btn != null)
            {
                btn.CambiarColorBase(normalColor);
            }
        }
    }

    // Click de los botones del menú radial
    public void OnLookClicked()
    {
        // Ahora el puente gestiona la ejecución
        itemInteractive.EjecutarAccion(currentItem, "Ver");
        HideMenu();
    }

    public void OnTouchClicked()
    {
        itemInteractive.EjecutarAccion(currentItem, "Tocar");
        HideMenu();
    }

    public void OnTalkClicked()
    {
        itemInteractive.EjecutarAccion(currentItem, "Hablar");
        HideMenu();
    }
}