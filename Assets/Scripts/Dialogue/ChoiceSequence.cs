using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Choice
{
    public string text;
    public DialogueAct branch;   // played when picked; null = just continue after the choice
}

[CreateAssetMenu(menuName = "Dialogue/Choice Sequence")]
public class ChoiceSequence : DialogueSequence
{
    [TextArea(1, 3)] public string prompt;   // optional question shown above the buttons
    public List<Choice> choices = new();
}
