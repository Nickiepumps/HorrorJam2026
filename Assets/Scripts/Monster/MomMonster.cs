using Cysharp.Threading.Tasks;
using UnityEngine;

public enum MomMonsState
{
    None, Patrol, Chase, Attack
}

public class MomMonster : BaseMonster<MomMonsState>
{
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private MomMonsterBodyTrigger bodyTrigger;
    private bool detectPlayer = false;
    private PlayerController player;
    private Vector2 currentTargetPos;
    [SerializeField] private float delayState = 2; // millisecond to work with UniTask.Delay
    [SerializeField] private float chaseSpeed;
    [SerializeField] private Transform[] patrolWaypointsArr1 = new Transform[2];
    [SerializeField] private Transform[] patrolWaypointsArr2 = new Transform[2];
    [SerializeField] private Transform currentStartPatrolWaypoint;
    [SerializeField] private Transform currentFinalPatrolWaypoint;
    private Transform goalWaypoint;

    protected override void Start()
    {
        playerDetector.OnDetectPlayer += PlayerDetector_OnDetectPlayer;
        playerDetector.OnPlayerOutOfDetect += PlayerDetector_OnPlayerOutOfDetect;

        ChangeCurrentWaypoints(patrolWaypointsArr1);
        base.Start();
    }

    private void PlayerDetector_OnPlayerOutOfDetect(Vector2 lastSeenPlayerPos)
    {
        Dev.Log();
        currentTargetPos = lastSeenPlayerPos;
        detectPlayer = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void PlayerDetector_OnDetectPlayer(PlayerController player)
    {
        detectPlayer = true;
        this.player = player;
        currentTargetPos = this.player.GetPlayerPosition();
        ChangeState(MomMonsState.Chase);
    }

    protected override void SetupStateHandler()
    {
        stateEnterHandlers = new()
        {
            { MomMonsState.Attack, () => OnAttackState().Forget() },
            { MomMonsState.Patrol, OnEnterPatrolState},
            { MomMonsState.Chase, OnEnterChaseState}
        };
    }

    private void OnEnterPatrolState()
    {
        animator.Play("Idle");
        goalWaypoint = currentStartPatrolWaypoint;
    }

    private void OnEnterChaseState()
    {
        animator.Play("Idle");
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        animator.Play("Attack");
        await UniTask.Delay((int)(delayState * 1000));
        ChangeState(MomMonsState.Chase);
    }

    protected override void OnUpdateState(MomMonsState newState)
    {
        switch (newState)
        {
            case MomMonsState.Patrol:
                OnPatrolState();
                break;
            case MomMonsState.Chase:
                OnChaseState();
                break;
        }
    }

    private void ChangeCurrentWaypoints(Transform[] newWaypointsArr)
    {
        currentStartPatrolWaypoint = newWaypointsArr[0];
        currentFinalPatrolWaypoint = newWaypointsArr[1];
    }
    private void OnPatrolState()
    {
        if (Mathf.Abs(transform.position.x - currentFinalPatrolWaypoint.position.x) <= 0.5f)
        {
            goalWaypoint = currentStartPatrolWaypoint;
        }
        else if (Mathf.Abs(transform.position.x - currentStartPatrolWaypoint.position.x) <= 0.5f)
        {
            goalWaypoint = currentFinalPatrolWaypoint;
        }

        float newX = Mathf.MoveTowards(transform.position.x, goalWaypoint.position.x, patrolSpeed * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);

        Flip();

    }

    private void Flip()
    {
        float xDirection = (transform.position - goalWaypoint.position).normalized.x;
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
        spriteRenderer.flipX = isFacingRight;

    }


    private void OnChaseState()
    {
        if (player.IsHidden())
        {
            ChangeState(MomMonsState.Patrol);
            return;
        } 

        if (detectPlayer)
        {
            currentTargetPos = player.GetPlayerPosition();
        }
        
        float newX = Mathf.MoveTowards(transform.position.x, currentTargetPos.x, chaseSpeed * Time.deltaTime);
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
            if(detectPlayer)
                ChangeState(MomMonsState.Attack);
            else
                ChangeState(MomMonsState.Patrol);
        }

        Flip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interact_Transition") && collision.TryGetComponent<TriggerSectionTransition>(out var sectionTransition))
        {
            if (sectionTransition.CanEnemyEnter())
            {
                TryGoToNextSection(sectionTransition.GetTargetSpawnTransform()).Forget();
                ChangeCurrentWaypoints(patrolWaypointsArr2);
            }
        }
    }

    private async UniTask TryGoToNextSection(Transform newSection)
    {
        await UniTask.Delay(2000);
        transform.position = newSection.position;
    }
}
