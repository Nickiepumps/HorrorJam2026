using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
public class MapSubSectionController : MonoBehaviour
{
    private SceneLightController sceneLightController => GameController.instance.sceneLightController;
    [SerializeField] private TransitionController transitionController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineConfiner2D confinerCam;

    [SerializeField] private TriggerSectionTransition[] triggerSectionTransitionArr;
    [SerializeField] private List<TriggerSectionTransition> triggerSectionTransitionList;

    [SerializeField] private Transform transformPlayerStartPos;
    [SerializeField] private Collider2D camConfinerPlayerRoom;

    private SectionType currentSectionType = SectionType.Floor3; // temp section
    public SectionType GetSectionType => currentSectionType;
    private CancellationTokenSource cts;
    private CancellationToken token;
    private void Start()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;
        if (playerController == null)
        {
            Debug.LogError($"{nameof(MapSubSectionController)} on {name} needs a Player Controller.", this);
            return;
        }

        foreach (TriggerSectionTransition trigger in triggerSectionTransitionArr)
        {
            trigger.Initialize(this, playerController.PlayerInventoryController());
        }
    }
    public void RemoveAndAddNewTriggerSectionTransitionGroup(TriggerSectionTransition[] currentTriggerGroupArr)
    {
        triggerSectionTransitionList.Clear();
        foreach(TriggerSectionTransition trigger in currentTriggerGroupArr)
        {
            triggerSectionTransitionList.Add(trigger);
        }
    }
    public async UniTaskVoid StartTransitionSection(Transform newSectionSpawnPos, Collider2D camConfiner, SectionType type)
    {
        if (newSectionSpawnPos == null || camConfiner == null || confinerCam == null ||
            transitionController == null || playerController == null)
        {
            return;
        }

        playerController.TogglePlayerActiveStatus(false);
        await transitionController.StartFadeInTransition(Color.black, token);
        confinerCam.BoundingShape2D = camConfiner;
        playerController.transform.position = newSectionSpawnPos.position;
        await UniTask.Delay(1000);
        currentSectionType = type;
        sceneLightController.InitializeSectionLight();
        playerController.TogglePlayerActiveStatus(true);
        await transitionController.StartFadeOutTransition(Color.black, token);
    }
    public void MoveToPlayerRoomSection()
    {
        playerController.transform.position = transformPlayerStartPos.transform.position;
        confinerCam.BoundingShape2D = camConfinerPlayerRoom;
        currentSectionType = SectionType.Floor5;
    }
}
