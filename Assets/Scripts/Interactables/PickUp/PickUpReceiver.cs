using UnityEngine;
public class PickUpReciever : MonoBehaviour, IRecievePickUp
{
    [SerializeField] protected float interactTimer;
    [SerializeField] protected BasePickUp pickUpTarget;
    public BasePickUp GetPickUpTarget => pickUpTarget;

    protected PickUpType typeTarget;
    private float currentInteractTime;
    protected bool isInteracting;
    protected bool isFinishedKeyInteract;
    protected virtual void StartInteract()
    {
        isInteracting = true;
        currentInteractTime = interactTimer;
    }
    protected virtual void CancelInteract()
    {
        isInteracting = false;
        currentInteractTime = interactTimer;
    }
    protected virtual void Update()
    {
        if (isFinishedKeyInteract == true) return;

        if (isInteracting == true)
        {
            currentInteractTime -= Time.deltaTime;
            if(currentInteractTime <= 0)
            {
                isFinishedKeyInteract = true;
                OnFinishedInteract();
            }
        }
    }
    protected virtual void OnFinishedInteract() { }
    public void OnReceivePickup(BasePickUp target)
    {
        if(target == pickUpTarget && isInteracting == false)
        {
            StartInteract();
        }
    }
    public void OnCancelInteract()
    {
        CancelInteract();
    }
}
