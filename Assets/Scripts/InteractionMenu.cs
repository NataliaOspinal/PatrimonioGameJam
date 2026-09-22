using UnityEngine;

public class InteractionMenu : MonoBehaviour
{
    public ClickManager clickManager;
    private ItemData currentItem;

    // Muestra el menú donde está el cursor
    public void ShowMenu(ItemData item, Vector2 screenPosition)
    {
        currentItem = item;
        transform.position = screenPosition;
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    // Funciones on cli

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