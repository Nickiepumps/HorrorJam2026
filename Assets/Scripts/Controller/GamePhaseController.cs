using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

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
public class GamePhaseController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private TransitionController transitionController;
    #region Debug
    // Debug Only
    [SerializeField] private Button buttonChangePhase;
    [SerializeField] private Button buttonWinPhase;
    [SerializeField] private Button buttonLosePhase;
    [SerializeField] private int debugPhase;
    #endregion

    [SerializeField] private InteractableGroup[] phaseInteractableGroupArr;
    [SerializeField] private BodyPartChoiceGroup[] bodyPartChoiceGroupArr;

    private int currentPhase;
    public void Initialize()
    {
        buttonChangePhase.onClick.AddListener(ChangePhase);
        buttonWinPhase.onClick.AddListener(() => WinPhase().Forget());
        buttonLosePhase.onClick.AddListener(LosePhase);
        currentPhase = 0;
        debugPhase = currentPhase;
    }
    public async UniTaskVoid NextPhase()
    {
        gameController.ResetPlayerPosition();
        await UniTask.Delay(3000);
        await transitionController.StartNextPhaseTransition();

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
        await transitionController.StartWinPhaseFadeIn(Color.black);
        dialogueController.StartWinPhaseDialogue(currentPhase).Forget();
    }
    public void LosePhase()
    {

    }
    private void ChangePhase()
    {
        if (debugPhase >= phaseInteractableGroupArr.Length) return;
        currentPhase = debugPhase;
        phaseInteractableGroupArr[debugPhase - 1].DeactivateInteractables();
        phaseInteractableGroupArr[debugPhase].ActivateInteractables();
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
