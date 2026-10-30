using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using TMPro;

[Serializable]
public class InteractableGroup
{
    public GameObject[] interactableArr;
    public void DeactivateInteractables()
    {
        foreach(GameObject interactable in interactableArr)
        {
            interactable.SetActive(false);
        }
    }
    public void ActivateInteractables()
    {
        foreach (GameObject interactable in interactableArr)
        {
            interactable.SetActive(true);
        }
    }
}
[Serializable]
public class BodyPartChoiceGroup
{
    public BodyPartSO[] mainChoiceBodyPartSO;
    public BodyPartSO[] choice3RandombodyPartSOArr;
    public BodyPartSO GetRandomizedBodyPart()
    {
        return choice3RandombodyPartSOArr[UnityEngine.Random.Range(0, choice3RandombodyPartSOArr.Length)];
    }
}
[Serializable]
public class SectionTransitionGroup
{
    public TriggerSectionTransition[] triggerSectionTransitionArr;
}
public class GamePhaseController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private TransitionController transitionController;
    private PlayerBodyPartController playerBodyPartController => GameController.instance.playerBodyPartController;
    private PhaseTimerController phaseTimerController => GameController.instance.phaseTimerController;
    private SceneLightController sceneLightController;
    #region Debug
    // Debug Only
    [SerializeField] private Button buttonChangePhase;
    [SerializeField] private Button buttonWinPhase;
    [SerializeField] private Button buttonEndingPhase;
    [SerializeField] private Button buttonLosePhase;
    [SerializeField] private int debugPhase;
    #endregion

    [SerializeField] private InteractableGroup[] phaseInteractableGroupArr;
    [SerializeField] private BodyPartChoiceGroup[] bodyPartChoiceGroupArr;
    [SerializeField] private SectionTransitionGroup[] sectionTransitionGroupArr;
    [SerializeField] private Sprite[] spriteLosePhaseCGrr;
    [SerializeField] private CanvasGroup canvasGroupCGDisplay;
    [SerializeField] private Image imageCGDisplay;
    [SerializeField] private TMP_Text textIntroMenu;

    [SerializeField] private AudioClip audioLose;
    [SerializeField] private AudioClip audioWin;
    [SerializeField] private AudioSource audioSource;

    private int currentPhase;
    public int GetPhase => currentPhase;
    public TriggerSectionTransition[] GetCurrentTriggerTransitionGroup => sectionTransitionGroupArr[currentPhase].triggerSectionTransitionArr;
    public void Initialize()
    {
        sceneLightController = gameController.sceneLightController;

        buttonChangePhase.onClick.AddListener(ChangePhase);
        buttonWinPhase.onClick.AddListener(() => WinPhase().Forget());
        buttonLosePhase.onClick.AddListener(() => LosePhase().Forget());
        buttonEndingPhase.onClick.AddListener(() => EndingPhase().Forget());
        currentPhase = 0;
        debugPhase = currentPhase;
    }
    public void GetCurrentBodyChoices()
    {
        // Get the choice based on player current body part condition
    }
    private async UniTaskVoid StartIntroPhase()
    {
        transitionController.InitializeIntro();
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
    }
    public async UniTaskVoid NextPhase()
    {
        currentPhase++;
        /*if (currentPhase >= phaseInteractableGroupArr.Length)
        {
            // Show Retry menu
            return;
        }*/
        if(currentPhase == 1)
        {
            playerBodyPartController.ToggleFlashLightBodyPart(true);
        }
        gameController.ResetPlayerPosition();
        gameController.playerController.ResetPlayerHP();
        gameController.mapSubSectionController.RemoveAndAddNewTriggerSectionTransitionGroup(GetCurrentTriggerTransitionGroup);
        await UniTask.Delay(3000);
        sceneLightController.InitializeSectionLight();
        gameController.sceneAudioController.ToggleSceneAudio(true);
        await transitionController.StartNextPhaseTransition();
        phaseTimerController.StartTimer(currentPhase);

        // Un comment this when assign all loop
        /*currentPhase++;
        if (currentPhase >= phaseInteractableGroupArr.Length) return;
        gameController.ResetPlayerPosition();
        phaseInteractableGroupArr[currentPhase - 1].DeactivateInteractables();
        phaseInteractableGroupArr[currentPhase].ActivateInteractables();
        await transitionController.StartNextPhaseTransition();*/
    }
    public async UniTaskVoid WinPhase()
    {
        gameController.sceneAudioController.ToggleSceneAudio(false);
        audioSource.clip = audioWin;
        audioSource.Play();
        await transitionController.StartPhaseCut(Color.black);
        dialogueController.StartWinPhaseDialogue(currentPhase).Forget();
    }
    public async UniTaskVoid EndingPhase()
    {
        gameController.sceneAudioController.ToggleSceneAudio(false);
        audioSource.clip = audioWin;
        audioSource.Play();
        await transitionController.StartPhaseFadeIn(Color.black);
        dialogueController.StartEndingPhaseDialogue().Forget();
    }
    public async UniTaskVoid LosePhase()
    {
        gameController.sceneAudioController.ToggleSceneAudio(false);
        audioSource.clip = audioLose;
        audioSource.Play();
        await transitionController.StartPhaseCut(Color.black);
        canvasGroupCGDisplay.alpha = 0f;
        imageCGDisplay.sprite = spriteLosePhaseCGrr[currentPhase];
        await Tween.Alpha(canvasGroupCGDisplay, 0f, 1f, 1f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(3000);
        await Tween.Alpha(canvasGroupCGDisplay, 1f, 0f, 1f, Easing.Standard(Ease.Linear));
        canvasGroupCGDisplay.alpha = 0f;
        dialogueController.StartLosePhaseDialogue(currentPhase).Forget();
    }
    private void ChangePhase()
    {
        if (debugPhase >= phaseInteractableGroupArr.Length) return;
        currentPhase = debugPhase;
        phaseInteractableGroupArr[debugPhase - 1].DeactivateInteractables();
        phaseInteractableGroupArr[debugPhase].ActivateInteractables();
        gameController.sceneAudioController.ToggleSceneAudio(true);
    }
    public BodyPartSO[] GetNextPhaseBodyPartChoice()
    {
        BodyPartSO[] resultChoiceGroupArr = new BodyPartSO[3];
        for(int i = 0; i < resultChoiceGroupArr.Length; i++)
        {
            if(i < 2)
            {
                resultChoiceGroupArr[i] = bodyPartChoiceGroupArr[currentPhase].mainChoiceBodyPartSO[i];
            }
            else
            {
                resultChoiceGroupArr[i] = bodyPartChoiceGroupArr[currentPhase].GetRandomizedBodyPart();
            }
        }
        return resultChoiceGroupArr;
    }
}
