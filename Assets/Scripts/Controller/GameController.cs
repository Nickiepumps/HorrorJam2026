using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerController playerController;
    public PlayerInventoryController playerInventoryController;
    public GamePhaseController gamePhaseController;

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
}
