using UnityEngine;
public class BasePushable : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D pushableCollider;

    [SerializeField] private Transform positionRightSidePush;
    [SerializeField] private Transform positionLeftSidePush;

    public TriggerPushable currentPlayerEnteredTrigger;
    private float xValue;
    public float GetXPositionValue => xValue;
    private void Start()
    {
        playerController = GameController.instance.playerController;
        Physics2D.IgnoreCollision(pushableCollider, playerController.PlayerCollider, true);
    }
    public void TogglePushInteract(bool state, Transform triggerPos)
    {
        if(state == true)
        {
            xValue = triggerPos.position.x - rb.position.x;
            if (xValue < 0)
            {
                playerController.transform.position = new Vector2(positionLeftSidePush.position.x, playerController.transform.position.y);
            }
            else
            {
                playerController.transform.position = new Vector2(positionRightSidePush.position.x, playerController.transform.position.y);
            }
            Physics2D.IgnoreCollision(pushableCollider, playerController.PlayerCollider, false);
        }
        else
        {
            Physics2D.IgnoreCollision(pushableCollider, playerController.PlayerCollider, true);
        }
    }
}
