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
    Climb
}
public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private Rigidbody2D rbPlayer;
    [SerializeField] private Animator animatorPlayer;
    [SerializeField] private LayerMask jumpLayer;

    private Collider2D currentEnteredCollider;
    private BaseHideable currentEnteredHideable;

    private bool isJump;
    private bool isHidden;
    private bool isOnGround;
    private bool isActive;
    private bool canMove;

    private float inputValue;
    private void Start()
    {
        isActive = true;
        currentState = PlayerState.Idle;
    }
    private void Update()
    {
        if (isActive == false) return;

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
        }
    }
    private void FixedUpdate()
    {
        if(isActive == false) return;

        switch (currentState)
        {
            case PlayerState.Idle:
                //StateIdleUpdate();
                break;
            case PlayerState.Run:
                StateRunFixUpdate();
                break;
            case PlayerState.Walk:
                StateWalkFixUpdate();
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
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
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
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            isOnGround = false;
        }
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
        }
    }
    #endregion
    #region Idle
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
    }
    private void StateIdleEnter()
    {
        if (isOnGround == true)
        {
            rbPlayer.linearVelocity = new Vector2(inputValue, rbPlayer.linearVelocity.y);
        }
        animatorPlayer.SetBool("IsIdle", true);
        animatorPlayer.SetBool("IsRun", false);
    }
    private void StateIdleExit()
    {

    }
    private void StateIdleTriggerEnter(Collider2D collision)
    {

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
        animatorPlayer.SetBool("IsIdle", true);
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
        // To Do: Show player character in front or on the side of the hideable object
        isHidden = false;
        spritePlayer.enabled = true;
        TogglePlayerMovement(true);
        currentEnteredHideable.Unhide();
    }
    #endregion
    #region Dead
    private void StateDeadEnter()
    {
        Debug.Log("Player Dead");
    }
    #endregion
}
