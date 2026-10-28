using UnityEngine;

public class InteractableLoot : PickUpReciever
{
    [SerializeField] private Collider2D lootCollider;
    [SerializeField] private BasePickUp lootedPickUp;
    private PlayerInventoryController playerInventoryController;
    private void Start()
    {
        typeTarget = PickUpType.Hand;
        playerInventoryController = GameController.instance.playerInventoryController;
    }
    protected override void OnFinishedInteract()
    {
        if (lootedPickUp == null)
        {
            gameObject.SetActive(false);
            return; // Display "nothing" on UI
        }
        playerInventoryController.AddPickUp(lootedPickUp);
        gameObject.SetActive(false);
    }
}
