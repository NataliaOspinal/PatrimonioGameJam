using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI textoObjetivos;
    private string objetivoActual = "";

    void Start()
    {
        if (textoObjetivos != null) textoObjetivos.text = "";
    }

    public void MostrarObjetivo(string nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
        ActualizarUI();
    }

    public void CompletarObjetivo()
    {
        if (!string.IsNullOrEmpty(objetivoActual))
        {
            StartCoroutine(RutinaCompletar());
        }
    }

    private IEnumerator RutinaCompletar()
    {
        // Tacha el texto cuando se completa
        textoObjetivos.text = "<b>Objetivos:</b>\n- <s>" + objetivoActual + "</s>";

        yield return new WaitForSeconds(2f); // Espera 2 segundos para que el jugador lo lea y luego whoosh

        objetivoActual = "";
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (string.IsNullOrEmpty(objetivoActual))
        {
            textoObjetivos.text = "";
        }
        else
        {
            textoObjetivos.text = "<b>Objetivos:</b>\n- " + objetivoActual;
        }
    }

    // Tacha el objetivo actual, espera 2 segundos, y escribe el nuevo
    public void CambiarObjetivoSecuencial(string nuevoObjetivo)
    {
        StartCoroutine(RutinaCambiarSecuencial(nuevoObjetivo));
    }

    private IEnumerator RutinaCambiarSecuencial(string nuevoObjetivo)
    {
        // Tacha el objetivo anterior sooolo si existe
        if (!string.IsNullOrEmpty(objetivoActual))
        {
            textoObjetivos.text = "<b>Objetivos:</b>\n- <s>" + objetivoActual + "</s>";
            yield return new WaitForSeconds(2.5f);
        }

        // Asigna y muestra el nuevo
        objetivoActual = nuevoObjetivo;
        ActualizarUI();
    }
}