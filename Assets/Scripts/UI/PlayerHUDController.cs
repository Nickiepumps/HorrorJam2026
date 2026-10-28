using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupHUD;
    [SerializeField] private Image[] imageHPIconArr;
    [SerializeField] private Image[] imageInventorySlotArr;
    public void HideHUD()
    {
        Tween.Alpha(canvasGroupHUD, 0f, 0.3f, Easing.Standard(Ease.Linear));
    }
    public void ShowHUD()
    {
        Tween.Alpha(canvasGroupHUD, 1f, 0.3f, Easing.Standard(Ease.Linear));
    }
    public void DecreaseHP(int value)
    {

    }
    public void AddHP(int value)
    {

    }
    public void AddItem(Sprite itemIcon)
    {

    }
    public void RemoveItem(Sprite itemiCon)
    {

    }
}
