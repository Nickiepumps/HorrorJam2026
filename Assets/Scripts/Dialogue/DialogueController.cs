using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] DialogueUI ui;
    [SerializeField] float charsPerSecond = 40f;

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public bool IsRunning { get; private set; }

    [SerializeField] private DialogueAct tempStartAct;

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
        StartAct(tempStartAct);
    }
    public void StartAct(DialogueAct act)
    {
        if (IsRunning || act == null) return;

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
                    ui.ShowChoices(choice, pickedIndex => OnChoicePicked(choice, pickedIndex));
                    return;
            }
        }
        EndDialogue();
    }

    private void OnChoicePicked(ChoiceSequence choiceSequence, int pickedIndex)
    {
        isWaitingForChoice = false;

        DialogueAct branch = choiceSequence.choices[pickedIndex].branch;
        if (branch != null) 
        {
            PushAct(branch);   // plays next; the parent act resumes after it
        } 

        ShowNextSequence();
    }

    private void EndDialogue()
    {
        IsRunning = false;
        ui.Hide();
        OnDialogueEnded?.Invoke();
    }
}