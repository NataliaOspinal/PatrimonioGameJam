using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class Diapositiva
{
    public GameObject objetoA;
    public GameObject objetoB; 
}

public class CreditManager : MonoBehaviour
{
    [Header("Fase 1: Diapositivas (1 al 5)")]
    public Diapositiva[] diapositivas; 
    public float tiempoPorDiapositiva = 3f;
    public float tiempoFade = 1f;

    [Header("Fase 2: Scroll de Texto")]
    public GameObject fondoScroll; 
    public RectTransform textoLargo; 
    public RectTransform puntoMetaTexto;
    public float tiempoViajeScroll = 12f;
    
    

    [Header("Navegación")]
    public string nombreEscenaMenu = "00_MenuPrincipal";

    void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ReproducirBGM("tema lima");
        }

        PrepararObjeto(fondoScroll, false);
        if (textoLargo != null) PrepararObjeto(textoLargo.gameObject, false);
        
        for (int i = 0; i < diapositivas.Length; i++)
        {
            bool empezarVisible = (i == 0); 
            
            PrepararObjeto(diapositivas[i].objetoA, empezarVisible);
            PrepararObjeto(diapositivas[i].objetoB, empezarVisible);
        }

        StartCoroutine(SecuenciaCreditos());
    }


    private void PrepararObjeto(GameObject obj, bool empezarVisible)
    {
        if (obj == null) return;
        
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = empezarVisible ? 1f : 0f; 

        SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in sprites)
        {
            Color colorActual = sr.color;
            colorActual.a = empezarVisible ? 1f : 0f;
            sr.color = colorActual;
        }

        obj.SetActive(empezarVisible); 
    }

    private void EjecutarFade(GameObject obj, float valorFinalDeTransparencia)
    {
        if (obj == null) return;
        obj.SetActive(true);
        
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.DOFade(valorFinalDeTransparencia, tiempoFade);
        }

        SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in sprites)
        {
            sr.DOFade(valorFinalDeTransparencia, tiempoFade);
        }
    }

  
    private IEnumerator SecuenciaCreditos()
    {
        
        yield return new WaitForSeconds(tiempoPorDiapositiva);

        for (int i = 1; i < diapositivas.Length; i++)
        {
            EjecutarFade(diapositivas[i].objetoA, 1f); 
            EjecutarFade(diapositivas[i].objetoB, 1f);
            
            EjecutarFade(diapositivas[i-1].objetoA, 0f); 
            EjecutarFade(diapositivas[i-1].objetoB, 0f);

            yield return new WaitForSeconds(tiempoFade);
            
            // Apagamos la vieja para limpiar memoria
            if (diapositivas[i-1].objetoA != null) diapositivas[i-1].objetoA.SetActive(false);
            if (diapositivas[i-1].objetoB != null) diapositivas[i-1].objetoB.SetActive(false);

            yield return new WaitForSeconds(tiempoPorDiapositiva);
        }

        
        int ultima = diapositivas.Length - 1;
        
        EjecutarFade(diapositivas[ultima].objetoA, 0f); 
        EjecutarFade(diapositivas[ultima].objetoB, 0f);
        
        EjecutarFade(fondoScroll, 1f);
        if (textoLargo != null) EjecutarFade(textoLargo.gameObject, 1f);

        yield return new WaitForSeconds(tiempoFade);
        
        if (diapositivas[ultima].objetoA != null) diapositivas[ultima].objetoA.SetActive(false);
        if (diapositivas[ultima].objetoB != null) diapositivas[ultima].objetoB.SetActive(false);

        if (textoLargo != null && puntoMetaTexto != null)
        {
            textoLargo.DOAnchorPosY(puntoMetaTexto.anchoredPosition.y, tiempoViajeScroll).SetEase(Ease.Linear);
            yield return new WaitForSeconds(tiempoViajeScroll);
        }

        yield return new WaitForSeconds(1.5f); 

        SceneManager.LoadScene(nombreEscenaMenu);
    }
}