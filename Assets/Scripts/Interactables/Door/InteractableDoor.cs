using UnityEngine;

public class InteractableDoor : TriggerSectionTransition
{
    //[SerializeField] private Collider2D doorCollider;
    protected override void StartInteract()
    {
        //doorCollider.enabled = false;
    }
    public override void Transition()
    {
        if (pickUpTarget != null && playerInventory != null &&
            playerInventory.TryGetPickUpInventory(pickUpTarget) != null)
        {
            playerInventory.RemovePickUp(pickUpTarget);
            isLocked = false;
        }

        base.Transition();
    }
}
