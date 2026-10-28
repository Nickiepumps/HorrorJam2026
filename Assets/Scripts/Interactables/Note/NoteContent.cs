using UnityEngine;

[CreateAssetMenu(fileName = "NewNoteContent", menuName = "Scriptable Objects/Note Content")]
public class NoteContent : ScriptableObject
{
    public string title;
    [TextArea(3, 10)]
    public string description;
}
