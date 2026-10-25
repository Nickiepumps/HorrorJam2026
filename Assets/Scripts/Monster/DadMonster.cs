using UnityEngine;
using Cysharp.Threading.Tasks;

public enum DadMonsState
{
    None, Move, Attack
}
public class DadMonster : StateManager<DadMonsState>
{
    private string playerTag = "Player";
    [SerializeField] private Transform startWaypoint;
    [SerializeField] private Transform endWaypoint;
    private Transform goalWaypoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;
    protected override void SetupStateHandler()
    {
        goalWaypoint = startWaypoint;
        stateEnterHandlers = new()
        {
            { DadMonsState.Attack, () => OnAttackState().Forget() }
        };
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        //animator.Play("Attack");
        await UniTask.Delay(1000);
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
        if (Mathf.Abs(transform.position.x - endWaypoint.position.x) <= 0.5f)
        {
            goalWaypoint = startWaypoint;
        }
        else if (Mathf.Abs(transform.position.x - startWaypoint.position.x) <= 0.5f)
        {
            goalWaypoint = endWaypoint;
        }

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

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(lightTag))
    //    {
    //        currentDeathTimer += 1 * Time.deltaTime;
    //        if (currentDeathTimer >= maxDeathTimer)
    //        {
    //            ChangeState(BrotherMonsState.Fade);
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(lightTag))
    //    {
    //        currentDeathTimer = 0;
    //    }
    //}
}

