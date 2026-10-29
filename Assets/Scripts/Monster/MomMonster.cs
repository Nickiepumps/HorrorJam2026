using Cysharp.Threading.Tasks;
using UnityEngine;

public enum MomMonsState
{
    None, Idle, Patrol, Alert, Chase, Attack
}

public class MomMonster : BaseMonster<MomMonsState>
{
    private PlayerController playerController;
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private MonsterBodyTrigger bodyTrigger;
    private bool detectPlayer = false;
    private PlayerController player;
    private Vector2 currentTargetPos;
    [SerializeField] private float delayState = 2; // millisecond to work with UniTask.Delay
    [SerializeField] private float chaseSpeed;
    [SerializeField] private Transform[] patrolWaypointsArr1 = new Transform[2];
    [SerializeField] private Transform[] patrolWaypointsArr2 = new Transform[2];
    private Transform[] currentPatrolWaypointsArr = new Transform[2];
    private Transform goalWaypoint;
    [SerializeField] private float delaySectionTranslateTime = 1;

    protected override void Start()
    {
        playerDetector.OnDetectPlayer += PlayerDetector_OnDetectPlayer;
        playerDetector.OnPlayerOutOfDetect += PlayerDetector_OnPlayerOutOfDetect;

        bodyTrigger.OnTranslateToNewSection += (newSection) => BodyTrigger_OnTranslateToNewSection(newSection).Forget();

        ChangeCurrentWaypoints(patrolWaypointsArr1);
        playerController = GameController.instance.playerController;
        base.Start();
    }

    private async UniTask BodyTrigger_OnTranslateToNewSection(Transform newSection)
    {
        await UniTask.Delay((int)(delaySectionTranslateTime * 1000));
        transform.position = newSection.position;
        ChangeState(MomMonsState.Idle);
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
        ChangeState(MomMonsState.Alert);
    }

    protected override void SetupStateHandler()
    {
        stateEnterHandlers = new()
        {
            { MomMonsState.Idle, () => OnEnterIdleState().Forget()},
            { MomMonsState.Attack, () => OnAttackState().Forget() },
            { MomMonsState.Patrol, OnEnterPatrolState},
            { MomMonsState.Alert, () => OnEnterAlertState().Forget()},
            { MomMonsState.Chase, OnEnterChaseState}
        };
    }

    private async UniTaskVoid OnEnterIdleState()
    {
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAlert", false);
        //animator.Play("Idle");
        await UniTask.Delay(1000);
        if (detectPlayer)
        {
            ChangeState(MomMonsState.Chase);
        }
        else
        {
            ChangeState(MomMonsState.Patrol);
        }
        
    }

    private void OnEnterPatrolState()
    {
        animator.SetBool("IsAttack", false);
        //animator.Play("Idle");
        goalWaypoint = currentPatrolWaypointsArr[0];
    }

    private async UniTask OnEnterAlertState()
    {
        Dev.Log();
        animator.SetBool("IsAlert", true);
        //animator.Play("Alert");
        await UniTask.Delay((int)(delayStateTime * 1000));
        ChangeState(MomMonsState.Chase);
    }

    private void OnEnterChaseState()
    {
        //animator.Play("Idle");
        animator.SetBool("IsAlert", false);
        animator.SetBool("IsAttack", false);
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        animator.SetBool("IsAttack", true);
        animator.SetBool("IsAlert", false);
        //animator.Play("Attack");
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length / 2 * 1000));
        playerController.OnMonsterAttack(1);
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length / 2 * 1000));
        //await UniTask.Delay((int)(delayState * 1000));
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
        currentPatrolWaypointsArr[0] = newWaypointsArr[0];
        currentPatrolWaypointsArr[1] = newWaypointsArr[1];
    }
    private void OnPatrolState()
    {
        if (Mathf.Abs(transform.position.x - currentPatrolWaypointsArr[1].position.x) <= 0.5f)
        {
            goalWaypoint = currentPatrolWaypointsArr[0];
        }
        else if (Mathf.Abs(transform.position.x - currentPatrolWaypointsArr[0].position.x) <= 0.5f)
        {
            goalWaypoint = currentPatrolWaypointsArr[1];
        }

        float newX = Mathf.MoveTowards(transform.position.x, goalWaypoint.position.x, patrolSpeed * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);

        Flip(goalWaypoint.position);

    }

    private void Flip(Vector3 targetDirection)
    {
        float xDirection = (transform.position - targetDirection).normalized.x;
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

        Flip(currentTargetPos);

        float distance = Mathf.Abs(currentTargetPos.x - transform.position.x);
        if(distance <= 0.5f)
        {
            if(detectPlayer)
                ChangeState(MomMonsState.Attack);
            else
                ChangeState(MomMonsState.Idle);
        }

        
    }
}
