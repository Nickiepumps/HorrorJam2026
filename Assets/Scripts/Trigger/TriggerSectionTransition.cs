using UnityEngine;
public class TriggerSectionTransition : PickUpReciever
{
    private MapSubSectionController controller;
    [SerializeField] private Transform targetSpawnTransform;
    [SerializeField] private Collider2D targetCamConfiner;

    private bool isLocked;
    [SerializeField] private bool canEnemyEnter;
    public void Initialize(MapSubSectionController controller)
    {
        this.controller = controller;
        typeTarget = PickUpType.Key;
        interactTimer = 1f;
    }
    public void Transition()
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
