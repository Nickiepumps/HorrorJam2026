using UnityEngine;

public class InteractableWinDoor : InteractableDoor
{
    public override void Transition()
    {        
        if (pickUpTarget != null && playerInventory != null &&
            playerInventory.TryGetPickUpInventory(pickUpTarget) != null)
        {
            playerInventory.RemovePickUp(pickUpTarget);
            GameController.instance.gamePhaseController.WinPhase().Forget();
        }

    }
}
