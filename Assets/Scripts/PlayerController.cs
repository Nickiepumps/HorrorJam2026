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
    [SerializeField] private LayerMask jumpLayer;

    private Collider2D currentEnteredCollider;

    private bool isJump;
    private bool isOnGround;

    private float inputValue;
    private void Start()
    {
        currentState = PlayerState.Idle;
    }
    private void Update()
    {
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
        }
    }
    private void FixedUpdate()
    {
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
        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jump:
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
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
    }
    private void StateIdleEnter()
    {
        if (isOnGround == true)
        {
            rbPlayer.linearVelocity = new Vector2(inputValue, rbPlayer.linearVelocity.y);
        }
    }
    private void StateIdleExit()
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
    #region Dead
    private void StateDeadEnter()
    {
        Debug.Log("Player Dead");
    }
    #endregion
}
