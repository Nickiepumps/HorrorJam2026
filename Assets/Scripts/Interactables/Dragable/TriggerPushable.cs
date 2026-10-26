using UnityEngine;
public class TriggerPushable : MonoBehaviour
{
    [SerializeField] private BasePushable pushableObject;
    public BasePushable GetPushableObject => pushableObject;

    public void TogglePush(bool state)
    {
        pushableObject.TogglePushInteract(state, transform);
    }
}
