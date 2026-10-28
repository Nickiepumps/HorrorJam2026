using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action<Vector2> OnDetectPlayer;
    public event Action<Vector2> OnPlayerOutOfDetect;
    private string playerTag = "Player";

    public void Flip()
    {
        transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            OnDetectPlayer?.Invoke(collision.transform.position);
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
