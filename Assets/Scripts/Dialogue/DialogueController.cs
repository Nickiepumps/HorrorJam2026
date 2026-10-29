using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class DialogueController : MonoBehaviour
{
    private GameController gameController;

    [SerializeField] private Image spriteDeathMerchant;
    [SerializeField] private CanvasGroup canvasGroupDialogue;
    [SerializeField] private CanvasGroup canvasGroupDialogueBox;

    [SerializeField] DialogueUI ui;
    [SerializeField] float charsPerSecond = 40f;

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public bool IsRunning { get; private set; }

    [SerializeField] private DialogueAct tempStartAct;

    [SerializeField] private DialogueAct[] winPhaseDialogueArr;
    [SerializeField] private DialogueAct[] losePhaseDialogueArr;
    [SerializeField] private DialogueAct endingPhaseDialogue;
    [SerializeField] private DialogueAct confirmedBodyPartDialogue;


    // How far we are through one act.
    private struct ActProgress
    {
        public DialogueAct act;
        public int nextSequenceIndex;
        public bool IsFinished => nextSequenceIndex >= act.sequences.Count;
    }

    // The act being played is on top. Acts below it are parents (the ones whose choice
    // started the branch) and wait there until the branch finishes.
    private readonly Stack<ActProgress> actsInProgress = new();

    private bool isWaitingForChoice;
    private void Start()
    {
        //StartAct(tempStartAct);
        gameController = GameController.instance;
    }
    public void StartAct(DialogueAct act)
    {
        if (IsRunning || act == null) return;

        canvasGroupDialogueBox.alpha = 1;
        IsRunning = true;
        actsInProgress.Clear();
        PushAct(act);

        ui.Show();
        OnDialogueStarted?.Invoke();
        ShowNextSequence();
    }

    // Called on click / Space. First press completes the typewriter, second press moves on.
    public void Advance()
    {
        if (!IsRunning || isWaitingForChoice) return;

        if (ui.IsTyping)
        {
            ui.SkipTyping();
        }
        else
        {
            ShowNextSequence();
        }
    }
    private void Update()
    {
        if (IsRunning && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Advance();
        }
    }
    public async UniTaskVoid StartWinPhaseDialogue(int currentPhase)
    {
        canvasGroupDialogue.alpha = 1;
        canvasGroupDialogueBox.alpha = 0;
        spriteDeathMerchant.color = new Color(spriteDeathMerchant.color.r, spriteDeathMerchant.color.g, spriteDeathMerchant.color.b, 0);
        await Tween.Alpha(spriteDeathMerchant, 1f, 1f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(1000);
        StartAct(winPhaseDialogueArr[currentPhase]);
    }
    public async UniTaskVoid StartEndingPhaseDialogue()
    {
        canvasGroupDialogue.alpha = 1;
        canvasGroupDialogueBox.alpha = 0;
        spriteDeathMerchant.color = new Color(spriteDeathMerchant.color.r, spriteDeathMerchant.color.g, spriteDeathMerchant.color.b, 0);
        await Tween.Alpha(spriteDeathMerchant, 1f, 1f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(1000);
        StartAct(endingPhaseDialogue);
    }
    public async UniTaskVoid StartLosePhaseDialogue(int currentPhase)
    {
        canvasGroupDialogue.alpha = 1;
        canvasGroupDialogueBox.alpha = 0;
        spriteDeathMerchant.color = new Color(spriteDeathMerchant.color.r, spriteDeathMerchant.color.g, spriteDeathMerchant.color.b, 0);
        await Tween.Alpha(spriteDeathMerchant, 1f, 1f, Easing.Standard(Ease.Linear));
        await UniTask.Delay(1000);
        StartAct(losePhaseDialogueArr[currentPhase]);
    }
    private async UniTaskVoid StartDialogueFadeOut()
    {
        await Tween.Alpha(canvasGroupDialogue, 0f, 1f, Easing.Standard(Ease.Linear));
        EndDialogue();
        canvasGroupDialogue.alpha = 0;
        await UniTask.Delay(1000);
        gameController.gamePhaseController.NextPhase().Forget();
    }
    private void PushAct(DialogueAct act)
    {
        actsInProgress.Push(new ActProgress { act = act, nextSequenceIndex = 0 });
    }
    private void ShowNextSequence()
    {
        while (actsInProgress.Count > 0)
        {
            ActProgress currentAct = actsInProgress.Pop();

            if (currentAct.IsFinished) continue;

            DialogueSequence sequence = currentAct.act.sequences[currentAct.nextSequenceIndex];
            currentAct.nextSequenceIndex++;
            actsInProgress.Push(currentAct);

            switch (sequence)
            {
                case LineSequence line:
                    ui.ShowLine(line, charsPerSecond);
                    return;

                case ChoiceSequence choice:
                    isWaitingForChoice = true;
                    ui.ShowBodyPartChoices(gameController.gamePhaseController.GetNextPhaseBodyPartChoice(), OnChoiceSelected);
                    return;
            }
        }
        StartDialogueFadeOut().Forget();
    }
    private void OnChoiceSelected(BodyPartSO selectedBodyPart)
    {
        isWaitingForChoice = false;
        PushAct(confirmedBodyPartDialogue);
        ShowNextSequence();
        gameController.playerBodyPartController.UpdateSelectedBodyPart(selectedBodyPart);
    }
    private void EndDialogue()
    {
        IsRunning = false;
        ui.Hide();
        OnDialogueEnded?.Invoke();
    }
}