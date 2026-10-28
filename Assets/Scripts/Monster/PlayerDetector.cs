using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action<PlayerController> OnDetectPlayer;
    public event Action<Vector2> OnPlayerOutOfDetect;
    private string playerTag = "Player";

    public void Flip()
    {
        transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && collision.TryGetComponent<PlayerController>(out var playerController))
        {
            OnDetectPlayer?.Invoke(playerController);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            OnPlayerOutOfDetect?.Invoke(collision.transform.position);
        }
    }
}
