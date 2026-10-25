using Cysharp.Threading.Tasks;
using UnityEngine;

public enum BrotherMonsState
{
    None, Idle, Attack, Fade
}

public class BrotherMonster : StateManager<BrotherMonsState>
{
    private string lightTag = "Light";
    private string playerTag = "Player";
    [SerializeField] private float maxDeathTimer;
    [SerializeField] private float currentDeathTimer;
    //private int delayAfterAttack = 400;
    [SerializeField] private Animator animator;

    protected override void SetupStateHandler()
    {
        stateHandlers = new()
        {
            { BrotherMonsState.Idle, OnIdleState },
            { BrotherMonsState.Attack, () => OnAttackState().Forget() },
            { BrotherMonsState.Fade, OnFadeState },
        };
    }

    private void OnIdleState()
    {
        animator.Play("Idle");
    }

    private async UniTaskVoid OnAttackState()
    {
        animator.Play("Attack");
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
        ChangeState(BrotherMonsState.Fade);
    }

    private void OnFadeState()
    {
        animator.Play("Fade");
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    protected override void OnUpdateState(BrotherMonsState newState)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && CurrentState != BrotherMonsState.Fade)
        {
            ChangeState(BrotherMonsState.Attack);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(lightTag))
        {
            currentDeathTimer += 1 * Time.deltaTime;
            if(currentDeathTimer >= maxDeathTimer)
            {
                ChangeState(BrotherMonsState.Fade);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(lightTag))
        {
            currentDeathTimer = 0;
        }
    }
}
