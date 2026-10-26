using Cysharp.Threading.Tasks;
using UnityEngine;

public enum MomMonsState
{
    None, Patrol, Chase, Attack
}

public class MomMonster : StateManager<MomMonsState>
{
    private string playerTag = "Player";
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private Rigidbody2D rb2d;
    private bool isNearPlayer = false;
    private bool isFacingRight = false;
    private Vector2 currentTargetPos;
    [SerializeField] private float delayState = 2; // millisecond to work with UniTask.Delay
    [SerializeField] private float moveSpeed; 

    protected override void Start()
    {
        playerDetector.OnDetectPlayer += PlayerDetector_OnDetectPlayer;
        playerDetector.OnPlayerOutOfDetect += PlayerDetector_OnPlayerOutOfDetect;
        base.Start();
    }

    private void PlayerDetector_OnPlayerOutOfDetect(Vector2 lastSeenPlayerPos)
    {
        Dev.Log();
        currentTargetPos = lastSeenPlayerPos;
        isNearPlayer = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void PlayerDetector_OnDetectPlayer(Vector2 playerPos)
    {
        print("Detect Player");
        isNearPlayer = true;
        currentTargetPos = playerPos;
        ChangeState(MomMonsState.Chase);
    }

    protected override void SetupStateHandler()
    {
        stateEnterHandlers = new()
        {
            { MomMonsState.Attack, () => OnAttackState().Forget() }
        };
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        //animator.Play("Attack");
        await UniTask.Delay((int)(delayState * 1000));
        ChangeState(MomMonsState.Chase);
    }

    protected override void OnUpdateState(MomMonsState newState)
    {
        switch (newState)
        {
            case MomMonsState.Patrol:
                //OnDadMoveState();
                break;
            case MomMonsState.Chase:
                OnMomChaseState();
                break;
        }
    }

    private void OnMomChaseState(Transform target)
    {

    }

    private void OnMomChaseState()
    {
        float newX = Mathf.MoveTowards(transform.position.x, currentTargetPos.x, moveSpeed * Time.deltaTime);       
        transform.position = new Vector2(newX, transform.position.y);

        float xDirection = ((Vector2)transform.position - currentTargetPos).normalized.x;
        if (xDirection > 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            playerDetector.Flip();
        }
        else if (xDirection < 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            playerDetector.Flip();
        }

        float distance = Mathf.Abs(currentTargetPos.x - transform.position.x);
        if(distance <= 0.5f)
        {
            if(isNearPlayer)
                ChangeState(MomMonsState.Attack);
            else
                ChangeState(MomMonsState.None);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
          /*
           change from "chase player position" to "move to last seen position"
           if find player, chase them again
           if not, patrol
          */
        }
    }
}
