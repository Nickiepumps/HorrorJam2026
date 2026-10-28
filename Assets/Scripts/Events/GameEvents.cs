using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnPlayerGetKeyItem;

    public static void PlayerGetKeyItem()
    {
        OnPlayerGetKeyItem?.Invoke();
    }
}
