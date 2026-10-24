using UnityEngine;
public enum PlayerState
{
    None,
    Idle,
    Move,
    Jump,
    Dead,
    Stun,
    Climb
}
public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private Rigidbody2D rbPlayer;
    [SerializeField] private LayerMask jumpLayer;

    private Collider2D currentEnteredCollider;

    private bool isJump;
    private bool isOnGround;
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
            case PlayerState.Move:
                StateMoveUpdate();
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
            case PlayerState.Move:
                StateMoveFixUpdate();
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Move:
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
        currentEnteredCollider = collision.collider;        
        switch (currentState)
        {
            case PlayerState.Idle:
                StateIdleCollisionEnter(collision);
                break;
            case PlayerState.Move:
                StateMoveCollisionEnter(collision);
                break;
            case PlayerState.Jump:
                StateJumpCollisionEnter(collision);
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
            case PlayerState.Move:
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
            case PlayerState.Move:
                StateMoveEnter();
                break;
            case PlayerState.Jump:
                StateJumpEnter();
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
            case PlayerState.Move:
                StateMoveExit();
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
        walkSpeed = 0f;
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            ChangeState(PlayerState.Move);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround == true)
        {
            ChangeState(PlayerState.Jump);
        }
    }
    private void StateIdleEnter()
    {

    }
    private void StateIdleExit()
    {

    }
    private void StateIdleCollisionEnter(Collision2D collision)
    {
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            isOnGround = true;
        }
    }
    #endregion
    #region Move
    private void StateMoveEnter()
    {
        walkSpeed = 3f;
    }
    private void StateMoveUpdate()
    {
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
    }
    private void StateMoveFixUpdate()
    {
        float inputValue = Input.GetAxisRaw("Horizontal");
        rbPlayer.linearVelocity = new Vector2(inputValue * walkSpeed, rbPlayer.linearVelocity.y);
    }
    private void StateMoveExit()
    {
        rbPlayer.linearVelocity = new Vector2(0, rbPlayer.linearVelocity.y);
    }
    private void StateMoveCollisionEnter(Collision2D collision)
    {
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            isOnGround = true;
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
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            ChangeState(PlayerState.Move);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }
    private void StateJumpCollisionEnter(Collision2D collision)
    {
        if (collision.collider.includeLayers.value == jumpLayer)
        {
            isOnGround = true;
        }
    }
    #endregion
}
