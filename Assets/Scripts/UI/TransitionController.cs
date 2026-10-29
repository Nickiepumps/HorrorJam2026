using Cysharp.Threading.Tasks;
using PrimeTween;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class TransitionController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroupTranstion;
    [SerializeField] private Image imageFade;

    public void InitializeIntro()
    {
        canvasGroupTranstion.alpha = 1f;
        imageFade.color = Color.black;
        gameObject.SetActive(true);
    }
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
    public async UniTask StartNextPhaseTransition()
    {
        await Tween.Alpha(canvasGroupTranstion, 0f, 0.4f, Easing.Standard(Ease.Linear));
        gameObject.SetActive(false);
    }
    public async UniTask StartPhaseFadeIn(Color fadeColor)
    {
        canvasGroupTranstion.alpha = 0f;
        gameObject.SetActive(true);
        imageFade.color = fadeColor;
        await Tween.Alpha(canvasGroupTranstion, 1f, 0.5f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(1000);
    }
    public async UniTask StartPhaseCut(Color fadeColor)
    {
        imageFade.color = fadeColor;
        canvasGroupTranstion.alpha = 1f;
        gameObject.SetActive(true);
        await UniTask.Delay(2000);
    }
}
