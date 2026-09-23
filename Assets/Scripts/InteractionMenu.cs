using UnityEngine;

public class InteractionMenu : MonoBehaviour
{
    public ClickManager clickManager;
    private ItemData currentItem;

    [Header("Botones del Menú Radial")]
    public GameObject btnVer;
    public GameObject btnTocar;
    public GameObject btnHablar;
    public GameObject btnEntrarSalir;

    // Muestra el men� donde est� el cursor
    public void ShowMenu(ItemData item)
    {
        currentItem = item;

        if (btnVer != null) btnVer.SetActive(item.puedeVer);
        if (btnTocar != null) btnTocar.SetActive(item.puedeTocar);
        if (btnHablar != null) btnHablar.SetActive(item.puedeHablar);

        Vector3 nuevaPosicion = item.transform.position + new Vector3(0, 1.5f, 0);

        nuevaPosicion.z = 0f;

        transform.position = nuevaPosicion;        
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    // Funciones on click

    public void OnLookClicked()
    {
        // Ver
        Debug.Log("Viendo: " + currentItem.gameObject.name);
        HideMenu();
    }

    public void OnTouchClicked()
    {
        // Tocar
        clickManager.InteractWithItem(currentItem, "Tocar");
        HideMenu();
    }

    public void OnTalkClicked()
    {
        // Hablar
        clickManager.InteractWithItem(currentItem, "Hablar");
        HideMenu();
    }
}