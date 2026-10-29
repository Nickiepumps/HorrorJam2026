using Cysharp.Threading.Tasks;
using UnityEngine;
public enum PlayerState
{
    None,
    Idle,
    Run,
    Walk,
    Crawl,
    Jump,
    Dead,
    Stun,
    Hide,
    Push,
    KeyInteract,
}
public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [SerializeField] private PlayerInventoryController playerInventoryController;
    [SerializeField] private PlayerBodyPartController playerBodyPartController;
    [SerializeField] private PlayerHUDController playerHUDController;

    private float hpValue;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float crawlSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private Rigidbody2D rbPlayer;
    [SerializeField] private Collider2D colliderPlayer;
    [SerializeField] private BoxCollider2D boxColliderPlayer;
    [SerializeField] private Animator animatorPlayer;
    [SerializeField] private LayerMask jumpLayer;

    private Collider2D currentEnteredCollider;
    private BaseHideable currentEnteredHideable;
    private TriggerSectionTransition currentEnteredTransition;
    private TriggerPushable currentEnteredPushable;
    private PickUpReciever currentKeyInteractable;
    private BasePickUp currentEnteredPickUp;
    private BasePickUp pickUpTargetFromInventory;
    private MonsterNoise currentEnterNoiseArea;

    private Vector2 standColliderSize;
    private Vector2 standColliderOffset;
    private Vector2 crawlColliderOffset;
    private Vector2 crawlColliderSize;
    public Collider2D PlayerCollider => colliderPlayer;

    private bool isJump;
    public bool isCrawl;
    private bool isHidden;
    private bool isPush;
    private bool isOnGround;
    private bool isHurt;
    private bool isDead;

    private bool canMove;
    private bool isActive;
    private bool isInteracting;
    private bool isUsingPickUp;

    private float inputValue;
    private int pushValue;
    private float xFacingValue;

    public PlayerInventoryController PlayerInventoryController() => playerInventoryController;
    private void Start()
    {
        playerHUDController.Initialize();
        hpValue = 3;
        isActive = true;
        canMove = true;
        standColliderOffset = new Vector2(-0.65f, -0.526f);
        standColliderSize = new Vector2(0.79f, 3.362f);
        crawlColliderOffset = new Vector2(-0.28f, -1.546f);
        crawlColliderSize = new Vector2(1.64f, 1.32f);
        currentState = PlayerState.Idle;
    }
    private void Update()
    {
        if (isActive == false) return;

        if (currentEnterNoiseArea != null)
        {
            hpValue -= currentEnterNoiseArea.Damage * Time.deltaTime;
            //playerHUDController.DecreaseHP((int)hpValue);
        }

        if (Input.GetKeyDown(KeyCode.E) && currentEnteredTransition != null)
        {
            currentEnteredTransition.Transition();
        }

        if(inputValue > 0)
        {
            xFacingValue = 1f;
        }
        else if(inputValue < 0)
        {
            xFacingValue = -1f;
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
            case PlayerState.Crawl:
                StateCrawlUpdate();
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
            case PlayerState.KeyInteract:
                StateKeyInteractUpdate();
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
            case PlayerState.Crawl:
                StateCrawlFixUpdate();
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
        if(collision.tag == "NoiseDamage")
        {
            currentEnterNoiseArea = collision.GetComponent<MonsterNoise>();
            playerHUDController.ToggleShakeHPBar(true);
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
        if (collision.tag == "KeyInteract")
        {
            currentKeyInteractable = null;
        }
        if (collision.tag == "NoiseDamage")
        {
            currentEnterNoiseArea = null;
            playerHUDController.ToggleShakeHPBar(false);
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
            case PlayerState.Crawl:
                StateCrawlCollisionStay(collision);
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
    public void OnMonsterAttack(int damage)
    {
        hpValue -= damage;
        playerHUDController.DecreaseHP((int)hpValue);
        Debug.Log(hpValue);
    }
    #region Base
    public void TogglePlayerActiveStatus(bool state)
    {
        isActive = state;
    }
    public void ResetPlayerHP() => hpValue = 3;
    public void TogglePlayerMovement(bool state)
    {
        canMove = state;
    }
    public void UpdatePlayerBodyStatus()
    {
        if(isCrawl == true)
        {
            ChangeState(PlayerState.Crawl);
        }
    }
    public bool IsHidden() => isHidden;
    public Vector2 GetPlayerPosition() => transform.position;
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
            case PlayerState.Crawl:
                StateCrawlEnter();
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
            case PlayerState.KeyInteract:
                StateKeyInteractEnter();
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
            case PlayerState.Crawl:
                StateCrawlExit();
                break;
            case PlayerState.Jump:
                StateJumpExit();
                break;
            case PlayerState.Dead:
                StateDeadExit();
                break;
            case PlayerState.Hide:
                StateHideExit();
                break;
            case PlayerState.Push:
                StatePushExit();
                break;
            case PlayerState.KeyInteract:
                StateKeyInteractExit();
                break;
        }
    }
    #endregion
    #region Idle
    private void StateIdleEnter()
    {
        // For BodyPart Debug
        rbPlayer.bodyType = RigidbodyType2D.Kinematic;
        boxColliderPlayer.size = standColliderSize;
        boxColliderPlayer.offset = standColliderOffset;
        rbPlayer.bodyType = RigidbodyType2D.Dynamic;

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

        #region Bodypart Anim
        switch (xFacingValue)
        {
            case 1:
                if (playerBodyPartController.Is1Arm == true)
                {
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 3f); // Blind facing
                        break;
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 3f);
                        break;
                    }
                    animatorPlayer.SetFloat("BlendIdle", 0f);
                }
                else if (playerBodyPartController.Is2Arm == true)
                {
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 4f); // Blind facing
                        break;
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 4f);
                        break;
                    }
                    animatorPlayer.SetFloat("BlendIdle", 1f);
                }
                else
                {
                    animatorPlayer.SetFloat("BlendIdle", 0f);
                }
                break;
            case -1:
                if (playerBodyPartController.Is1Arm == true)
                {
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 0f); // Non-Blinded Eye facing
                        break;
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 3f);
                        break;
                    }
                    animatorPlayer.SetFloat("BlendIdle", 0f);
                }
                else if (playerBodyPartController.Is2Arm == true)
                {
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 1f); // Non-Blinded Eye facing
                        break;
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendIdle", 4f);
                        break;
                    }
                    animatorPlayer.SetFloat("BlendIdle", 1f);
                }
                else
                {
                    animatorPlayer.SetFloat("BlendIdle", 0f);
                }
                break;
        }
        #endregion
    }
    private void StateIdleUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        if (hpValue <= 0)
        {
            ChangeState(PlayerState.Dead);
        }
        if (isCrawl == true)
        {
            ChangeState(PlayerState.Crawl);
        }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            ChangeState(PlayerState.Walk);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround == true)
        {
            ChangeState(PlayerState.Jump);
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPickUp != null)
        {
            playerInventoryController.AddPickUp(currentEnteredPickUp);
            currentEnteredPickUp.gameObject.SetActive(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
            return;
        }
        if(Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentKeyInteractable != null && isOnGround == true)
        {
            ChangeState(PlayerState.KeyInteract);
            return;
        }
    }
    private void StateIdleExit()
    {
        animatorPlayer.SetBool("IsIdle", false);
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

        #region Bodypart Anim
        if (playerBodyPartController.IsToggleFlashLight == false)
        {
            animatorPlayer.SetFloat("BlendY", 0f);
        }
        else
        {
            animatorPlayer.SetFloat("BlendY", 5.5f);
        }
        switch (xFacingValue)
        {
            case 1:
                if (playerBodyPartController.Is1Arm == true)
                {
                    animatorPlayer.SetFloat("BlendY", 1f);
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 3f);
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 3.5f);
                    }
                    break;
                }
                if (playerBodyPartController.Is2Arm == true)
                {
                    animatorPlayer.SetFloat("BlendY", 0.5f);
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 2f);
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 2.5f);
                    }
                    break;
                }
                if (playerBodyPartController.Is1Eye == true)
                {
                    animatorPlayer.SetFloat("BlendY", 4f);
                }
                if (playerBodyPartController.Is2Eye == true)
                {
                    animatorPlayer.SetFloat("BlendY", 5f);
                }
                break;
            case -1:
                if (playerBodyPartController.Is1Arm == true)
                {
                    animatorPlayer.SetFloat("BlendY", 1.5f);
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 6f);
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 6.5f);
                    }
                    break;
                }
                if (playerBodyPartController.Is2Arm == true)
                {
                    animatorPlayer.SetFloat("BlendY", 0.5f);
                    if (playerBodyPartController.Is1Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 7f);
                    }
                    if (playerBodyPartController.Is2Eye == true)
                    {
                        animatorPlayer.SetFloat("BlendY", 2.5f);
                    }
                    break;
                }
                if (playerBodyPartController.Is1Eye == true)
                {
                    animatorPlayer.SetFloat("BlendY", 4.5f);
                }
                if (playerBodyPartController.Is2Eye == true)
                {
                    animatorPlayer.SetFloat("BlendY", 5f);
                }
                break;
        }
        #endregion

        walkSpeed = 3f;
    }
    private void StateWalkUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        if (hpValue <= 0)
        {
            ChangeState(PlayerState.Dead);
        }
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
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPickUp != null)
        {
            playerInventoryController.AddPickUp(currentEnteredPickUp);
            currentEnteredPickUp.gameObject.SetActive(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentKeyInteractable != null && isOnGround == true)
        {
            ChangeState(PlayerState.KeyInteract);
            return;
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
        if (hpValue <= 0)
        {
            ChangeState(PlayerState.Dead);
        }
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
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPickUp != null)
        {
            playerInventoryController.AddPickUp(currentEnteredPickUp);
            currentEnteredPickUp.gameObject.SetActive(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentKeyInteractable != null && isOnGround == true)
        {
            ChangeState(PlayerState.KeyInteract);
            return;
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
    }
    #endregion
    #region Crawl
    private void StateCrawlEnter()
    {
        // Play Crawl Anim
        crawlSpeed = 2f;
        rbPlayer.bodyType = RigidbodyType2D.Kinematic;
        boxColliderPlayer.size = crawlColliderSize;
        boxColliderPlayer.offset = crawlColliderOffset;
        rbPlayer.bodyType = RigidbodyType2D.Dynamic;
    }
    private void StateCrawlUpdate()
    {
        inputValue = Input.GetAxisRaw("Horizontal");
        if (hpValue <= 0)
        {
            ChangeState(PlayerState.Dead);
        }

        animatorPlayer.SetFloat("BlendX", xFacingValue);
        #region 2 Legs
        if (playerBodyPartController.Is2Leg == true)
        {
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 4f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 4.5f);
                    spritePlayer.flipX = false;
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 4.5f);
                    spritePlayer.flipX = true;
                    break;
            }
        }
        if (playerBodyPartController.Is2Leg == true && playerBodyPartController.Is1Eye == true)
        {
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 5f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 5.5f);
                    spritePlayer.flipX = false;
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 5.5f);
                    spritePlayer.flipX = true;
                    break;
            }
        }
        if (playerBodyPartController.Is2Leg == true && playerBodyPartController.Is2Eye == true)
        {
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 6f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 6.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 6.5f);
                    break;
            }
        }
        #endregion
        #region 1 leg
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is1Arm == false)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 0f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 0.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 0.5f);
                    break;
            }
        }
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is1Arm == true)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 1f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 1.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 1.5f);
                    break;
            }
        }
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is1Arm == true && playerBodyPartController.Is1Eye == true)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 2f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 2.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 2.5f);
                    break;
            }
        }
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is1Arm == true && playerBodyPartController.Is2Eye == true)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 3f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 3.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 3.5f);
                    break;
            }
        }
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is1Eye == true)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 7f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 7.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 7.5f);
                    break;
            }
        }
        if (playerBodyPartController.Is1Leg == true && playerBodyPartController.Is2Eye == true)
        {
            spritePlayer.flipX = false;
            switch (inputValue)
            {
                case 0:
                    animatorPlayer.SetFloat("BlendY", 8f);
                    break;
                case 1:
                    animatorPlayer.SetFloat("BlendY", 8.5f);
                    break;
                case -1:
                    animatorPlayer.SetFloat("BlendY", 8.5f);
                    break;
            }
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredHideable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Hide);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentEnteredPushable != null && isOnGround == true)
        {
            ChangeState(PlayerState.Push);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentKeyInteractable != null && isOnGround == true)
        {
            ChangeState(PlayerState.KeyInteract);
            return;
        }
        if (isCrawl == false)
        {
            ChangeState(PlayerState.Idle);
        }
    }
    private void StateCrawlFixUpdate()
    {
        rbPlayer.linearVelocity = new Vector2(inputValue * crawlSpeed, rbPlayer.linearVelocity.y);
    }
    private void StateCrawlCollisionStay(Collision2D collision)
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
    private void StateCrawlExit() { }
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
    #region KeyInteract
    private void StateKeyInteractEnter()
    {
        pickUpTargetFromInventory = playerInventoryController.TryGetPickUpInventory(currentKeyInteractable.GetPickUpTarget);
        isInteracting = true;
    }
    private void StateKeyInteractUpdate()
    {
        if (currentKeyInteractable == null)
        {
            ChangeState(PlayerState.Idle);
        }
        if (Input.GetKey(KeyCode.E) && currentKeyInteractable != null)
        {
            currentKeyInteractable.OnReceivePickup(pickUpTargetFromInventory);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }
    private void StateKeyInteractExit()
    {
        if (currentKeyInteractable != null)
        {
            currentKeyInteractable.OnCancelInteract();
        }
        isInteracting = false;
    }
    #endregion

    #region Dead
    private void StateDeadEnter()
    {
        Debug.Log("Dead");
        isActive = false;
    }
    private void StateDeadExit()
    {
        isActive = true;
    }
    #endregion
}