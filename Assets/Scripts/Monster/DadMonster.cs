using UnityEngine;
using Cysharp.Threading.Tasks;

public enum DadMonsState
{
    None, Idle, Patrol, Attack
}
public class DadMonster : BaseMonster<DadMonsState>
{
    private PlayerController playerController => GameController.instance.playerController;
    private SceneAudioController sceneAudioController => GameController.instance.sceneAudioController;

    [SerializeField] private Transform startWaypoint;
    [SerializeField] private Transform endWaypoint;
    [SerializeField] private AudioClip audioWalk;
    [SerializeField] private AudioClip audioAttack;
    [SerializeField] private AudioClip audioIdle;
    [SerializeField] private AudioSource audioSource;
    private Transform goalWaypoint;
    private int delayState = 1000; // millisecond to work with UniTask.Delay
    protected override void SetupStateHandler()
    {
        sceneAudioController.AddAudioSource(audioSource);
        goalWaypoint = startWaypoint;
        stateEnterHandlers = new()
        {
            { DadMonsState.Idle, () => OnEnterIdleState().Forget() },
            { DadMonsState.Patrol, OnEnterPatrolState},
            { DadMonsState.Attack, () => OnAttackState().Forget() }
        };
    }

    private async UniTaskVoid OnEnterIdleState()
    {
        animator.SetBool("IsAttack", false);
        audioSource.clip = audioIdle;
        audioSource.loop = true;
        audioSource.Play();
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

    private void OnEnterPatrolState()
    {
        animator.SetBool("IsAttack", false);
        audioSource.clip = audioWalk;
        audioSource.loop = true;
        audioSource.Play();
    }

    private async UniTaskVoid OnAttackState()
    {
        Dev.Log();
        animator.SetBool("IsAttack", true);
        //animator.Play("Attack");
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length / 2 * 1000));
        audioSource.clip = audioAttack;
        audioSource.loop = false;
        audioSource.Play();
        playerController.OnMonsterAttack(3);
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length / 2 * 1000));
        //await UniTask.Delay(delayState);
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
                if(playerController.IsDead == false)
                {
                    ChangeState(DadMonsState.Attack);
                }
                else
                {
                    audioSource.clip = audioWalk;
                    audioSource.loop = true;
                }
            }
            
        }
    }
}

