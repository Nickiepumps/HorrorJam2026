using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] GameObject root;

    [Header("Line")]
    [SerializeField] Image portrait;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dialogueText;

    [Header("Choices")]
    [SerializeField] GameObject choicePanel;
    [SerializeField] Transform choiceContainer;
    [SerializeField] Button choiceButtonPrefab;

    private Coroutine typing;
    public bool IsTyping { get; private set; }

    public void Show() => root.SetActive(true);
    public void Hide() 
    { 
        root.SetActive(false); 
        choicePanel.SetActive(false);
    }

    public void ShowLine(LineSequence line, float charsPerSecond)
    {
        choicePanel.SetActive(false);
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

    public void ShowChoices(ChoiceSequence choiceSequence, Action<int> onChoicePicked)
    {
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
            button.GetComponentInChildren<TMP_Text>().text = choiceSequence.choices[i].text;
            button.onClick.AddListener(() => onChoicePicked(choiceIndex));
        }
        choicePanel.SetActive(true);
    }
}
