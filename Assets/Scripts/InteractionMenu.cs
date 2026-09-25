using UnityEngine;
using UnityEngine.UI; // Necesario para los colores del hover

public class InteractionMenu : MonoBehaviour
{
    public ItemInteractuar itemInteractive;
    private ItemData currentItem;

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

        Vector3 nuevaPosicion = item.transform.position + new Vector3(0, 1.5f, 0);
        nuevaPosicion.z = 0f;
        transform.position = nuevaPosicion;

        gameObject.SetActive(true);
        ResetButtons(); // Asegura que ningún botón empiece oscurecido
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
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