using UnityEngine;
public class BasePickUp : MonoBehaviour
{
    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private Sprite spritePickUpItem;
    public Sprite GetSprite => spritePickUpItem;
}
