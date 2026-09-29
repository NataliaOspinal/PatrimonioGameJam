using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CambioMenu : MonoBehaviour
{
    public string nombreEscena = "99_Credits";

    public string nombrePrimerNivel = "01_EscenaInicial";

    [Header("Panel de Opciones")]
    public GameObject panelOpciones; 
    public float tiempoAnimacion = 0.4f;
    private bool opcionesAbiertas = false;

    private Vector3 escalaOriginal;

    void Start()
    {
        if (panelOpciones != null)
        {
            escalaOriginal = panelOpciones.transform.localScale; 
            
            panelOpciones.transform.localScale = Vector3.zero;
            panelOpciones.SetActive(false);
        }
    }

    public void BotonInicio()
    {
        SceneManager.LoadScene(nombrePrimerNivel);
    }

    public void BotonOpciones()
    {
        if (panelOpciones == null) return;

        opcionesAbiertas = !opcionesAbiertas;

        if (opcionesAbiertas)
        {
            panelOpciones.SetActive(true);
            panelOpciones.transform.DOScale(escalaOriginal, tiempoAnimacion).SetEase(Ease.OutBack);
        }
        else
        {
            panelOpciones.transform.DOScale(Vector3.zero, tiempoAnimacion).SetEase(Ease.InBack)
                .OnComplete(() => panelOpciones.SetActive(false));
        }
    }

    public void IrACreditos()
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void CambiarPantallaCompleta(bool esCompleta)
    {
        Screen.fullScreen = esCompleta;
    }
}
