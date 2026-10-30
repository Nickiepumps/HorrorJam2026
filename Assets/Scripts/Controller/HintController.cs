using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using PrimeTween;
public class HintController : MonoBehaviour
{
    private PhaseTimerController phaseTimerController => GameController.instance.phaseTimerController;

    [SerializeField] private CanvasGroup canvasGroupHintPanel;
    [SerializeField] private TMP_Text textHintDetail;
    public async UniTaskVoid ToggleHint(bool state, string hint = "")
    {
        if(state == true)
        {
            phaseTimerController.PauseTimer();
            canvasGroupHintPanel.alpha = 0f;
            textHintDetail.text = hint;
            await Tween.Alpha(canvasGroupHintPanel, 1f, 0.3f, Easing.Standard(Ease.Linear));
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            ToggleHint(false).Forget();
        }
        else
        {
            phaseTimerController.ResumeTimer();
            await Tween.Alpha(canvasGroupHintPanel, 0f, 0.3f, Easing.Standard(Ease.Linear));
            canvasGroupHintPanel.alpha = 0f;
        }
    }
}
