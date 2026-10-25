using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class MapSubSectionController : MonoBehaviour
{
    [SerializeField] private TransitionController transitionController;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private TriggerSectionTransition[] triggerSectionTransitionArr;
    private CancellationTokenSource cts;
    private CancellationToken token;
    private void Start()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;
        foreach(TriggerSectionTransition trigger in triggerSectionTransitionArr)
        {
            trigger.Initialize(this);
        }
    }
    public async UniTaskVoid StartTransitionSection(Transform newSectionSpawnPos)
    {
        playerController.TogglePlayerActiveStatus(false);
        await transitionController.StartFadeInTransition(Color.black, token);
        playerController.transform.position = newSectionSpawnPos.position;
        await UniTask.Delay(1000);
        playerController.TogglePlayerActiveStatus(true);
        await transitionController.StartFadeOutTransition(Color.black, token);
    }
}
