using UnityEngine;
using PrimeTween;
public class MonsterNoise : MonoBehaviour
{
    [SerializeField] private float maxDeathTimer;
    [SerializeField] private float currentDeathTimer;
    [SerializeField] private float damage;
    [SerializeField] private SpriteRenderer spriteObject;
    private string lightTag = "Light";
    public float Damage => damage;
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
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(lightTag))
        {
            currentDeathTimer = 0;
        }
    }
}
