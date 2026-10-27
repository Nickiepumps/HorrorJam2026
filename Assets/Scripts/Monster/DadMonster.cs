using UnityEngine;
using Cysharp.Threading.Tasks;

public enum DadMonsState
{
    None, Idle, Patrol, Attack
}
public class DadMonster : BaseMonster<DadMonsState>
{
    [SerializeField] private Transform startWaypoint;
    [SerializeField] private Transform endWaypoint;
    private Transform goalWaypoint;
    private int delayState = 1000; // millisecond to work with UniTask.Delay
    protected override void SetupStateHandler()
    {
        goalWaypoint = startWaypoint;
        stateEnterHandlers = new()
        {
            { DadMonsState.Idle, () => OnIdleState().Forget() },
            { DadMonsState.Attack, () => OnAttackState().Forget() }
        };
    }

    private async UniTaskVoid OnIdleState()
    {
        animator.Play("Idle");
        await UniTask.Delay(delayState);
        if (goalWaypoint == endWaypoint)
        {
            goalWaypoint = startWaypoint;
        }
        else
        {
            goalWaypoint = endWaypoint;
        }
        ChangeState(DadMonsState.Patrol);
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        animator.Play("Attack");
        await UniTask.Delay(delayState);
        ChangeState(DadMonsState.Idle);
    }

    protected override void OnUpdateState(DadMonsState newState)
    {
        switch (newState)
        {
            case DadMonsState.Patrol:
                OnPatrolState();
                break;
        }
    }
    private void Flip()
    {
        float xDirection = (transform.position - goalWaypoint.position).normalized.x;
        if (xDirection > 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
        }
        else if (xDirection < 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
        }
        spriteRenderer.flipX = isFacingRight;
    }

    private void OnPatrolState()
    {
        if ((Mathf.Abs(transform.position.x - goalWaypoint.position.x) <= 0.5f))
        {
            ChangeState(DadMonsState.Idle);
            return;
        }
        //animator.Play("Move");

        float newX = Mathf.MoveTowards(transform.position.x, goalWaypoint.position.x, 2 * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);

        Flip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && collision.TryGetComponent<PlayerController>(out var playerController))
        {
            if (!playerController.IsHidden())
            {
                ChangeState(DadMonsState.Attack);
            }
            
        }
    }
}

