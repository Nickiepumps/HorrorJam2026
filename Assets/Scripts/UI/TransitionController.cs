using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Threading;
public class TransitionController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupTranstion;
    [SerializeField] private Image imageFade;

    public async UniTask StartFadeInTransition(Color fadeColor, CancellationToken cts)
    {
        canvasGroupTranstion.alpha = 0f;
        gameObject.SetActive(true);
        imageFade.color = fadeColor;
        await Tween.Alpha(canvasGroupTranstion, 1f, 0.4f, Easing.Standard(Ease.Linear)).SetCancellationToken(cts);
    }
    public async UniTask StartFadeOutTransition(Color fadeColor, CancellationToken cts)
    {
        imageFade.color = fadeColor;
        await Tween.Alpha(canvasGroupTranstion, 0f, 0.4f, Easing.Standard(Ease.Linear)).SetCancellationToken(cts);
        gameObject.SetActive(false);
    }
    public async UniTask StartWinPhaseFadeIn(Color fadeColor)
    {
        canvasGroupTranstion.alpha = 0f;
        gameObject.SetActive(true);
        imageFade.color = fadeColor;
        await Tween.Alpha(canvasGroupTranstion, 1f, 0.5f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(1000);
    }
}
