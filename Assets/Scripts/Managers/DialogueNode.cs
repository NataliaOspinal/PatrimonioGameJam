using UnityEngine;
using System.Collections.Generic;

// se puede crear los nodos con click derecho en una carpeta 
[CreateAssetMenu(fileName = "NuevoNodo", menuName = "Dialogos/Nuevo Nodo")]
public class DialogueNode : ScriptableObject
{
    [TextArea(3, 10)]
    public string textoNPC; // el dialogo del personaje

    public List<OpcionDialogo> opciones; // respuestas de las opciones
}

[System.Serializable]
public class OpcionDialogo
{
    public string textoJugador;
    public DialogueNode siguienteNodo; // Si esta vacio termina el dialogo 
}