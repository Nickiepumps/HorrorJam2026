using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueType
{
    None, Normal, Choice
}

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Speaker Portrait")]
    [SerializeField] private Image portrait;
    [SerializeField] private Transform middlePos;
    [SerializeField] private Transform rightPos;

    [Header("Line")]
    [SerializeField] private GameObject linePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Choices")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private Coroutine typing;
    public bool IsTyping { get; private set; }

    public void Show() => root.SetActive(true);
    public void Hide() 
    { 
        root.SetActive(false); 
    }

    IEnumerator Type(string fullText, float charsPerSecond)
    {
        IsTyping = true;
        dialogueText.text = fullText;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;
        float charactersRevealed = 0f;
        while (dialogueText.maxVisibleCharacters < totalCharacters)
        {
            charactersRevealed += Time.unscaledDeltaTime * charsPerSecond;
            dialogueText.maxVisibleCharacters = Mathf.Min(totalCharacters, (int)charactersRevealed);
            yield return null;
        }
        IsTyping = false;
    }

    public void SkipTyping()
    {
        if (typing != null) StopCoroutine(typing);
        dialogueText.maxVisibleCharacters = int.MaxValue;
        IsTyping = false;
    }
    public void ShowLine(LineSequence line, float charsPerSecond)
    {
        portrait.transform.position = middlePos.position;
        portrait.gameObject.SetActive(line.speaker && line.speaker.sprite);
        if (line.speaker)
        {
            portrait.sprite = line.speaker.sprite;
            nameText.text = line.speaker.displayName;
            nameText.color = line.speaker.nameColor;
        }
        else nameText.text = "";

        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(line.text, charsPerSecond));
        ChangeDialogueView(DialogueType.Normal);
    }

    public void ShowChoices(ChoiceSequence choiceSequence, Action<int> onChoicePicked)
    {
        portrait.transform.position = rightPos.position;
        if (typing != null) StopCoroutine(typing);
        IsTyping = false;

        nameText.text = "";
        dialogueText.text = choiceSequence.prompt;
        dialogueText.maxVisibleCharacters = int.MaxValue;

        foreach (Transform oldButton in choiceContainer) Destroy(oldButton.gameObject);
        for (int i = 0; i < choiceSequence.choices.Count; i++)
        {
            int choiceIndex = i; // capture for the lambda
            var button = Instantiate(choiceButtonPrefab, choiceContainer);
            //button.GetComponentInChildren<TMP_Text>().text = choiceSequence.choices[i].text;
            button.onClick.AddListener(() => onChoicePicked(choiceIndex));
        }
        ChangeDialogueView(DialogueType.Choice);


    }

    private void ChangeDialogueView(DialogueType type)
    {
        switch (type)
        {
            case DialogueType.Normal:
                linePanel.SetActive(true);
                choicePanel.SetActive(false);
                break;
            case DialogueType.Choice:
                linePanel.SetActive(false);
                choicePanel.SetActive(true);
                break;
        }
    }
}
