using UnityEngine;
using UnityEngine.EventSystems; 
public class HoverSello : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sello")]
    public GameObject objetoSello;

    void Start()
    {
        if (objetoSello != null)
        {
            objetoSello.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (objetoSello != null)
        {
            objetoSello.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (objetoSello != null)
        {
            objetoSello.SetActive(false);
        }
    }
}
