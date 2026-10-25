using System.Runtime.CompilerServices;
using UnityEngine;

public static class Dev
{
    public static void Log([CallerMemberName] string memberName = "")
    {
        Debug.Log($"{memberName}()");
    }
}
