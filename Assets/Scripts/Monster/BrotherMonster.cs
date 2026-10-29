using Cysharp.Threading.Tasks;
using UnityEngine;

public enum BrotherMonsState
{
    None, Idle, Attack, Fade
}

public class BrotherMonster : BaseMonster<BrotherMonsState>
{
    private PlayerController playerController => GameController.instance.playerController;
    private SceneAudioController sceneAudioController => GameController.instance.sceneAudioController;
    [SerializeField] private float maxDeathTimer;
    [SerializeField] private float currentDeathTimer;

    [SerializeField] private AudioClip audioIdle;
    [SerializeField] private AudioClip audioAttack;
    [SerializeField] private AudioClip audioFade;
    [SerializeField] private AudioSource audioSource;
    protected override void SetupStateHandler()
    {
        sceneAudioController.AddAudioSource(audioSource);
        stateEnterHandlers = new()
        {
            { BrotherMonsState.Idle, OnIdleState },
            { BrotherMonsState.Attack, () => OnAttackState().Forget() },
            { BrotherMonsState.Fade, OnFadeState },
        };
    }

    private void OnIdleState()
    {
        animator.Play("Idle");
        audioSource.clip = audioIdle;
        audioSource.loop = true;
        audioSource.Play();
    }

    private async UniTaskVoid OnAttackState()
    {
        animator.Play("Attack");
        audioSource.clip = audioAttack;
        audioSource.loop = false;
        audioSource.Play();
        await UniTask.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
        playerController.OnMonsterAttack(1);
        ChangeState(BrotherMonsState.Fade);
    }

    private void OnFadeState()
    {
        animator.Play("Fade");
        audioSource.clip = audioFade;
        audioSource.loop = false;
        audioSource.Play();
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
