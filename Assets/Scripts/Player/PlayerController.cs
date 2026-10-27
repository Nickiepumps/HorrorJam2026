using UnityEngine;
public enum PlayerState
{
    None,
    Idle,
    Run,
    Walk,
    Jump,
    Dead,
    Stun,
    Hide,
    Push
}
public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [SerializeField] private PlayerInventoryController playerInventoryController;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private Rigidbody2D rbPlayer;
    [SerializeField] private Collider2D colliderPlayer;
    [SerializeField] private Animator animatorPlayer;
    [SerializeField] private LayerMask jumpLayer;

    private Collider2D currentEnteredCollider;
    private BaseHideable currentEnteredHideable;
    private TriggerSectionTransition currentEnteredTransition;
    private TriggerPushable currentEnteredPushable;
    private PickUpReciever currentKeyInteractable;
    private BasePickUp currentEnteredPickUp;
    private BasePickUp pickUpTargetFromInventory;
    
    public Collider2D PlayerCollider => colliderPlayer;

    private bool isJump;
    private bool isHidden;
    private bool isPush;
    private bool isOnGround;

    private bool canMove;
    private bool isActive;
    private bool isInteracting;
    private bool isUsingPickUp;

    private float inputValue;
    private int pushValue;
    private void Start()
    {
        isActive = true;
        currentState = PlayerState.Idle;
    }
    private void Update()
    {
        if (isActive == false) return;

        if (Input.GetKeyDown(KeyCode.E) && currentEnteredTransition != null)
        {
            currentEnteredTransition.Transition();
        }
        if(currentKeyInteractable != null && isInteracting == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pickUpTargetFromInventory = playerInventoryController.TryGetPickUpInventory(currentKeyInteractable.GetPickUpTarget);
            }
            if (Input.GetKey(KeyCode.E))
            {
                currentKeyInteractable.OnReceivePickup(pickUpTargetFromInventory); // Temp
            }
            else
            {
                currentKeyInteractable.OnCancelInteract();
            }
        }
        if(Input.GetKeyDown(KeyCode.E) && currentEnteredPickUp != null)
        {
            playerInventoryController.AddPickUp(currentEnteredPickUp);
            currentEnteredPickUp.gameObject.SetActive(false);
        }

        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleUpdate();
                break;
            case PlayerState.Run:
                StateRunUpdate();
                break;
            case PlayerState.Walk:
                StateWalkUpdate();
                break;
            case PlayerState.Jump:
                StateJumpUpdate();
                break;
            case PlayerState.Hide:
                StateHideUpdate();
                break;
            case PlayerState.Push:
                StatePushUpdate();
                break;
        }
    }
    private void FixedUpdate()
    {
        if(isActive == false) return;

        switch (currentState)
        {
            case PlayerState.Run:
                StateRunFixUpdate();
                break;
            case PlayerState.Walk:
                StateWalkFixUpdate();
                break;
            case PlayerState.Push:
                StatePushFixUpdate();
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive == false) return;

        if(collision.tag == "Interact_Hide")
        {
            currentEnteredHideable = collision.GetComponent<BaseHideable>();
        }
        if (collision.tag == "Interact_Transition")
        {
            currentEnteredTransition = collision.GetComponent<TriggerSectionTransition>();
        }
        if(collision.tag == "Interact_Drag")
        {
            currentEnteredPushable = collision.GetComponent<TriggerPushable>();
        }
        if (collision.tag == "Interact_PickUp")
        {
            currentEnteredPickUp = collision.GetComponent<BasePickUp>();
        }
        if (collision.tag == "KeyInteract")
        {
            currentKeyInteractable = collision.GetComponent<PickUpReciever>();
        }

        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleTriggerEnter(collision);
                break;
            case PlayerState.Walk:
                StateWalkTriggerEnter(collision);
                break;
            case PlayerState.Run:
                StateRunTriggerEnter(collision);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Interact_Hide")
        {
            currentEnteredHideable = null;
        }
        if (collision.tag == "Interact_Transition")
        {
            currentEnteredTransition = null;
        }
        if (collision.tag == "Interact_Drag")
        {
            currentEnteredPushable = null;
        }
        if (collision.tag == "Interact_PickUp")
        {
            currentEnteredPickUp = null;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleCollisionEnter(collision);
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jump:
                StateJumpCollisionEnter(collision);
                break;
            case PlayerState.Walk:
                StateWalkCollisionStay(collision);
                break;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        currentEnteredCollider = collision.collider;      
        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleCollisionStay(collision);
                break;
            case PlayerState.Run:
                StateRunCollisionStay(collision);
                break;
            case PlayerState.Jump:
                StateJumpCollisionStay(collision);
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jump:
                StateJumpCollisionExit(collision);
                break;
        }
    }
    public void TogglePlayerActiveStatus(bool state)
    {
        isActive = state;
    }
    public void TogglePlayerMovement(bool state)
    {
        canMove = state;
    }

    public bool IsHidden() => isHidden;
    public Vector2 GetPlayerPosition() => transform.position;
    #region Base
    private void ChangeState(PlayerState newState)
    {
        Exit();
        currentState = newState;
        Enter();
    }
    private void Enter()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleEnter();
                break;
            case PlayerState.Run:
                StateRunEnter();
                break;
            case PlayerState.Walk:
                StateWalkEnter();
                break;
            case PlayerState.Jump:
                StateJumpEnter();
                break;
            case PlayerState.Hide:
                StateHideEnter();
                break;
            case PlayerState.Push:
                StatePushEnter();
                break;
            case PlayerState.Dead:
                StateDeadEnter();
                break;
        }
    }
    private void Exit()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleExit();
                break;
            case PlayerState.Run:
                StateRunExit();
                break;
            case PlayerState.Jump:
                StateJumpExit();
                break;
            case PlayerState.Hide:
                StateHideExit();
                break;
            case PlayerState.Push:
                StatePushExit();
                break;
        }
    }
    #endregion
    #region Idle
    private void StateIdleEnter()
    {
        if (isOnGround == true)
        {
            rbPlayer.linearVelocity = new Vector2(inputValue, rbPlayer.linearVelocity.y);
            animatorPlayer.SetBool("IsIdle", true);
        }
        else
        {
            animatorPlayer.SetBool("IsIdle", false);
        }
        animatorPlayer.SetBool("IsWalk", false);
        animatorPlayer.SetBool("IsRun", false);
    }
    private void StateIdleUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            ChangeState(PlayerState.Walk);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround == true)
        {
            ChangeState(PlayerState.Jump);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
        }
        if(Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
        }
    }
    private void StateIdleExit()
    {

    }
    private void StateIdleTriggerEnter(Collider2D collision)
    {

    }
    private void StateIdleCollisionEnter(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            if (surfaceNormal.x == 0)
            {
                animatorPlayer.SetBool("IsIdle", true);
            }
        }
    }
    private void StateIdleCollisionStay(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            if (surfaceNormal.x < 0 || surfaceNormal.x > 0)
            {
                isOnGround = false;
            }
            else
            {
                isOnGround = true;
            }
        }
    }
    #endregion
    #region Walk
    private void StateWalkEnter()
    {
        animatorPlayer.SetBool("IsIdle", false);
        animatorPlayer.SetBool("IsWalk", true);
        animatorPlayer.SetBool("IsRun", false);
        walkSpeed = 3f;
    }
    private void StateWalkUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        switch (inputValue)
        {
            case 0:
                ChangeState(PlayerState.Idle);
                break;
            case 1:
                spritePlayer.flipX = false;
                break;
            case -1:
                spritePlayer.flipX = true;
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround == true)
        {
            ChangeState(PlayerState.Jump);
        }
        if (Input.GetKey(KeyCode.LeftShift) && isOnGround == true)
        {
            ChangeState(PlayerState.Run);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
        }
    }
    private void StateWalkFixUpdate()
    {
        if (isOnGround == false && inputValue == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else
        {
            rbPlayer.linearVelocity = new Vector2(inputValue * walkSpeed, rbPlayer.linearVelocity.y);
        }
    }
    private void StateWalkTriggerEnter(Collider2D collision)
    {

    }
    private void StateWalkCollisionStay(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            if (surfaceNormal.x < 0 || surfaceNormal.x > 0)
            {
                isOnGround = false;
            }
            else
            {
                isOnGround = true;
            }
        }
    }
    #endregion
    #region Run
    private void StateRunEnter()
    {
        runSpeed = 5f;
        animatorPlayer.SetBool("IsIdle", false);
        animatorPlayer.SetBool("IsWalk", false);
        animatorPlayer.SetBool("IsRun", true);
    }
    private void StateRunUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        switch (Input.GetAxisRaw("Horizontal"))
        {
            case 0:
                ChangeState(PlayerState.Idle);
                break;
            case 1:
                spritePlayer.flipX = false;
                break;
            case -1:
                spritePlayer.flipX = true;
                break;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround == true)
        {
            ChangeState(PlayerState.Jump);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isOnGround == true)
        {
            ChangeState(PlayerState.Walk);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
        }
    }
    private void StateRunFixUpdate()
    {
        if(isOnGround == false && inputValue == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else
        {
            rbPlayer.linearVelocity = new Vector2(inputValue * runSpeed, rbPlayer.linearVelocity.y);
        }
    }
    private void StateRunExit()
    {
        
    }
    private void StateRunTriggerEnter(Collider2D collision)
    {

    }
    private void StateRunCollisionStay(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            /*if (surfaceNormal.x < 0 || surfaceNormal.x > 0)
            {
                isOnGround = false;
            }
            else
            {
                isOnGround = true;
            }*/
        }
    }
    #endregion
    #region Jump
    private void StateJumpEnter()
    {
        isJump = true;
        rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    private void StateJumpExit()
    {
        isJump = false;
    }
    private void StateJumpUpdate()
    {
        
    }
    private void StateJumpCollisionStay(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            if (surfaceNormal.x < 0 || surfaceNormal.x > 0)
            {
                isOnGround = false;
            }
            else
            {
                isOnGround = true;
                isJump = false;
            }
        }
    }
    private void StateJumpCollisionEnter(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.GetContact(0).normal;
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            if (surfaceNormal.y > 0)
            {
                isOnGround = true;
            }
        }
    }
    private void StateJumpCollisionExit(Collision2D collision)
    {
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            isJump = false;
            ChangeState(PlayerState.Idle);
        }
    }
    #endregion
    #region Hide
    private void StateHideEnter()
    {
        // Disable player sprite, collider, and movement
        isInteracting = true;
        isHidden = true;
        TogglePlayerMovement(false);
        spritePlayer.enabled = false;
        currentEnteredHideable.Hide();
    }
    private void StateHideUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeState(PlayerState.Idle);
        }
    }
    private void StateHideExit()
    {
        isInteracting = false;
        isHidden = false;
        spritePlayer.enabled = true;
        TogglePlayerMovement(true);
        currentEnteredHideable.Unhide();
    }
    #endregion
    #region Push
    private void StatePushEnter()
    {
        currentEnteredPushable.TogglePush(true);
        isInteracting = true;
        isPush = true;
    }
    private void StatePushExit()
    {
        currentEnteredPushable.TogglePush(false);
        pushValue = 0;
        isInteracting = false;
        isPush = false;
    }
    private void StatePushUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeState(PlayerState.Idle);
        }
        switch (inputValue)
        {
            case -1:
                if (currentEnteredPushable.GetPushableObject.GetXPositionValue > 0)
                {
                    spritePlayer.flipX = true;
                    animatorPlayer.SetBool("IsIdle", false);
                    animatorPlayer.SetBool("IsWalk", true);
                    animatorPlayer.SetBool("IsRun", false);
                    pushValue = -1;
                }
                else
                {
                    animatorPlayer.SetBool("IsIdle", true);
                    animatorPlayer.SetBool("IsWalk", false);
                    animatorPlayer.SetBool("IsRun", false);
                    pushValue = 0;
                }
                break;
            case 1:
                if (currentEnteredPushable.GetPushableObject.GetXPositionValue < 0)
                {
                    spritePlayer.flipX = false;
                    animatorPlayer.SetBool("IsIdle", false);
                    animatorPlayer.SetBool("IsWalk", true);
                    animatorPlayer.SetBool("IsRun", false);
                    pushValue = 1;
                }
                else
                {
                    animatorPlayer.SetBool("IsIdle", true);
                    animatorPlayer.SetBool("IsWalk", false);
                    animatorPlayer.SetBool("IsRun", false);
                    pushValue = 0;
                }
                break;
            case 0:
                animatorPlayer.SetBool("IsIdle", true);
                animatorPlayer.SetBool("IsWalk", false);
                animatorPlayer.SetBool("IsRun", false);
                pushValue = 0;
                break;
        }
    }
    private void StatePushFixUpdate()
    {
        rbPlayer.linearVelocity = new Vector2(pushValue * walkSpeed, rbPlayer.linearVelocity.y);
    }
    #endregion
    #region Dead
    private void StateDeadEnter()
    {
        Debug.Log("Player Dead");
    }
    #endregion
}
