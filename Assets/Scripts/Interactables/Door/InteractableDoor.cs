using UnityEngine;

public class InteractableDoor : PickUpReciever
{
    [SerializeField] private Collider2D doorCollider;
    protected override void StartInteract()
    {
        doorCollider.enabled = false;
    }
}
