using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Line Sequence")]
public class LineSequence : DialogueSequence
{
    public CharacterData speaker;
    [TextArea(2, 6)] public string text;
}
