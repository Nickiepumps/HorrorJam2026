using UnityEngine;
using Cysharp.Threading.Tasks;

public enum DadMonsState
{
    None, Idle, Move, Attack
}
public class DadMonster : StateManager<DadMonsState>
{
    private string playerTag = "Player";
    [SerializeField] private Transform startWaypoint;
    [SerializeField] private Transform endWaypoint;
    private Transform goalWaypoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;
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
        ChangeState(DadMonsState.Move);
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        animator.Play("Attack");
        await UniTask.Delay(delayState);
        ChangeState(DadMonsState.Move);
    }

    protected override void OnUpdateState(DadMonsState newState)
    {
        switch (newState)
        {
            case DadMonsState.Move:
                OnDadMoveState();
                break;
        }
    }

    private void OnDadMoveState()
    {
        if ((Mathf.Abs(transform.position.x - goalWaypoint.position.x) <= 0.5f))
        {
            ChangeState(DadMonsState.Idle);
            return;
        }
        animator.Play("Move");

        float newX = Mathf.MoveTowards(transform.position.x, goalWaypoint.position.x, 2 * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) /*&& CurrentState != BrotherMonsState.Fade*/)
        {
            ChangeState(DadMonsState.Attack);
        }
    }
}

