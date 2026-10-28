using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NoteReaderPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI noteReaderTMP;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private void Start()
    {
        GameEvents.OnPlayerPickUpNote += (noteContent) => GameEvents_OnPlayerPickUpNote(noteContent).Forget();
    }

    private async UniTaskVoid GameEvents_OnPlayerPickUpNote(string noteContent)
    {
        if (isShowing == true) return;
        canvasGroup.alpha = 1;
        noteReaderTMP.text = noteContent;
        isShowing = true;
        await UniTask.Delay(5000);
        isShowing = false;
        canvasGroup.alpha = 0;
    }
}
