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
}