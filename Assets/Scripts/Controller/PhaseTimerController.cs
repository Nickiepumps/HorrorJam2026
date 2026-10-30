using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PhaseTimerController : MonoBehaviour
{
    private GamePhaseController gamePhaseController => GameController.instance.gamePhaseController;

    [SerializeField] private CanvasGroup canvasGroupTimerDisplay;
    [SerializeField] private TMP_Text textTimer;
    [SerializeField] private Image imageTimerIcon;

    [SerializeField] private float[] timerPhaseArr;

    private float currentTime;
    private bool toggleTimer;
    private void Start()
    {
        canvasGroupTimerDisplay.alpha = 1;
    }
    public void Update()
    {
        if(toggleTimer == true)
        {
            if (currentTime <= 0)
            {
                toggleTimer = false;
                gamePhaseController.LosePhase().Forget();
            }
            currentTime -= Time.deltaTime;
            textTimer.text = ((int)currentTime).ToString();
        }
    }
    public void StartTimer(int phaseTarget)
    {
        if (phaseTarget == 0)
        {
            canvasGroupTimerDisplay.alpha = 0;
        }
        else
        {
            canvasGroupTimerDisplay.alpha = 1;
            currentTime = timerPhaseArr[phaseTarget];
            toggleTimer = true;
        }
    }
    public void PauseTimer()
    {
        toggleTimer = false;
    }
    public void ResumeTimer()
    {
        toggleTimer = true;
    }
}
