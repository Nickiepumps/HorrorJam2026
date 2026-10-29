using UnityEngine;
public class TriggerSectionTransition : PickUpReciever
{
    protected MapSubSectionController controller;
    protected PlayerInventoryController playerInventory;
    [SerializeField] protected Transform targetSpawnTransform;
    [SerializeField] protected Collider2D targetCamConfiner;

    protected bool isLocked = false;
    [SerializeField] protected bool canEnemyEnter;
    public void Initialize(MapSubSectionController controller, PlayerInventoryController playerInventory)
    {
        this.controller = controller;
        this.playerInventory = playerInventory;
        typeTarget = PickUpType.Key;
        interactTimer = 1f;
    }
    public virtual void Transition()
    {
        if (isLocked == true)
        {
            
        }
        else
        {
            controller.StartTransitionSection(targetSpawnTransform, targetCamConfiner).Forget();
        }
    }
    protected override void OnFinishedInteract()
    {
        base.OnFinishedInteract();
        isLocked = false;
    }
    protected override void CancelInteract()
    {
        base.CancelInteract();
        isLocked = true;
    }

    public Transform GetTargetSpawnTransform() => targetSpawnTransform;

    public bool CanEnemyEnter() => canEnemyEnter;
}
