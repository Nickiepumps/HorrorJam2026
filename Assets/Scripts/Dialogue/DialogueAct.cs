using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Act")]
public class DialogueAct : ScriptableObject
{
    public List<DialogueSequence> sequences = new();
}
