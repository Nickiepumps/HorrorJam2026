using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerController playerController;
    public PlayerInventoryController playerInventoryController;
    public PlayerBodyPartController playerBodyPartController;
    public GamePhaseController gamePhaseController;
    public MapSubSectionController mapSubSectionController;
    public SceneLightController sceneLightController;
    public SceneAudioController sceneAudioController;
    public PhaseTimerController phaseTimerController;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }
    private void Start()
    {
        gamePhaseController.Initialize();
    }
    public void ResetPlayerPosition()
    {
        mapSubSectionController.MoveToPlayerRoomSection();
    }
}
