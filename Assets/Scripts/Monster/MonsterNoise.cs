using UnityEngine;
using PrimeTween;
public class MonsterNoise : MonoBehaviour
{
    private PlayerController playerController => GameController.instance.playerController;
    [SerializeField] private float maxDeathTimer;
    [SerializeField] private float currentDeathTimer;
    private float damage = 0.5f;
    [SerializeField] private SpriteRenderer spriteObject;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip clipIdle;
    private string lightTag = "Light";
    public float Damage => damage;
    private float damageInterval = 2f;
    private float currentDamageTime;
    private bool isPlayerEnter;

    private void Start()
    {
        animator.Play(clipIdle.name);
    }
    private void Update()
    {
        if (isPlayerEnter == true && playerController.GetHP > 0)
        {
            currentDamageTime += Time.deltaTime;
            if (currentDamageTime >= damageInterval)
            {
                playerController.OnMonsterAttack(damage);
                currentDamageTime = 0;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(lightTag))
        {
            currentDeathTimer += 1 * Time.deltaTime;
            if (currentDeathTimer >= maxDeathTimer)
            {
                Tween.Alpha(spriteObject, 0f, 0.5f, Easing.Standard(Ease.Linear));
            }
        }
        if(collision.tag == "Player")
        {
            if (playerController.GetHP <= 0)
            {
                isPlayerEnter = false;
                return;
            }
            isPlayerEnter = true;
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
