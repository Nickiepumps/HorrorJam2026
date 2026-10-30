using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupHUD;
    [SerializeField] private CanvasGroup canvasGroupHPBar;
    [SerializeField] private RectTransform panelHPBar;
    [SerializeField] private Image[] imageHPIconArr;
    [SerializeField] private Sprite imageHPFullIcon;
    [SerializeField] private Sprite imageHPDepletedIcon;
    [SerializeField] private Sprite imageHPHalfIcon;
    [SerializeField] private Image[] imageInventorySlotArr;

    private Tween tweenFlickerHP;
    public void Initialize()
    {
        
    }
    public void HideHUD()
    {
        Tween.Alpha(canvasGroupHUD, 0f, 0.3f, Easing.Standard(Ease.Linear));
    }
    public void ShowHUD()
    {
        Tween.Alpha(canvasGroupHUD, 1f, 0.3f, Easing.Standard(Ease.Linear));
    }
    public void ToggleShakeHPBar(bool status)
    {
        if (status == true)
        {
            tweenFlickerHP = Tween.Alpha(canvasGroupHPBar, 1f, 0f, 0.3f, Easing.Standard(Ease.OutCubic), -1, CycleMode.Yoyo);
        }
        else
        {
            canvasGroupHPBar.alpha = 1f;
            tweenFlickerHP.Complete();
        }
    }
    public void DecreaseHP(float value)
    {
        if (value < 0) return;
        if((int)value == value - 0.5f)
        {
            imageHPIconArr[(int)value].sprite = imageHPHalfIcon;   
        }
        else
        {
            imageHPIconArr[(int)value].sprite = imageHPDepletedIcon;
        }
    }
    public void ResetHP()
    {
        foreach(Image icon in imageHPIconArr)
        {
            icon.sprite = imageHPFullIcon;
        }
    }
    public void AddItem(Sprite itemIcon)
    {

    }
    public void RemoveItem(Sprite itemiCon)
    {

    }
}
