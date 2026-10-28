using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerController playerController;
    public PlayerInventoryController playerInventoryController;
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
}
