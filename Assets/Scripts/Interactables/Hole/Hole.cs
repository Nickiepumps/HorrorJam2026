using UnityEngine;

public class Hole : TriggerSectionTransition
{
    [SerializeField] private GameObject filledHoleVisual;
    [SerializeField] private Collider2D holeCollider;

    public override void Transition()
    {
        if (isLocked)
        {
            return;
        }

        if (pickUpTarget != null && playerInventory != null &&
            playerInventory.TryGetPickUpInventory(pickUpTarget) != null)
        {
            playerInventory.RemovePickUp(pickUpTarget);
            isLocked = true;

            if (filledHoleVisual != null)
            {
                filledHoleVisual.SetActive(true);
            }

            if (holeCollider != null)
            {
                holeCollider.enabled = false;
            }

            return;
        }

        base.Transition();
    }
}
