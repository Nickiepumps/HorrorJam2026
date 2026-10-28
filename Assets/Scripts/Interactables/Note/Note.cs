using UnityEngine;

public class Note : BasePickUp
{
    public NoteContent noteContents;
    public override void OnPickUp()
    {
        Dev.Log();
        GameEvents.PlayerPickUpNote(noteContents.description);
    }
}
