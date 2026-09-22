using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioMenu : MonoBehaviour
{
    public string nombreEscena = "99_Credits";

    public string nombrePrimerNivel = "01_EscenaInicial";

    public void BotonInicio()
    {
        SceneManager.LoadScene(nombrePrimerNivel);
    }

    public void BotonOpciones()
    {
        Debug.Log("Botón de opciones presionado");
    }

    public void IrACreditos()
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
