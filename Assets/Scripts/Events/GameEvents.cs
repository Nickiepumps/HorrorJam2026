using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerGetKeyItem;
    public static event Action<string> OnPlayerPickUpNote;

    public static void PlayerGetKeyItem()
    {
        OnPlayerGetKeyItem?.Invoke();
    }

    public static void PlayerPickUpNote(string noteContent)
    {
        OnPlayerPickUpNote?.Invoke(noteContent);
    }
}
