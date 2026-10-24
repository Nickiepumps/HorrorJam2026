using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class MapSubSectionController : MonoBehaviour
{
    [SerializeField] private TransitionController transitionController;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private TriggerSectionTransition[] triggerSectionTransitionArr;
    private CancellationToken cts;
    private void Start()
    {
        foreach(TriggerSectionTransition trigger in triggerSectionTransitionArr)
        {
            trigger.Initialize(this);
        }
    }
    public async UniTaskVoid StartTransitionSection(Transform newSectionSpawnPos)
    {
        cts = new CancellationToken();
        playerController.TogglePlayerMovement(false);
        await transitionController.StartFadeInTransition(Color.black, cts);
        playerController.transform.position = newSectionSpawnPos.position;
        await UniTask.Delay(1000);
        playerController.TogglePlayerMovement(true);
        await transitionController.StartFadeOutTransition(Color.black, cts);
        cts.ThrowIfCancellationRequested();
    }
}
