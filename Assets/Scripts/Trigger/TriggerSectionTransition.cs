using UnityEngine;

public class TriggerSectionTransition : MonoBehaviour
{
    private MapSubSectionController controller;
    [SerializeField] private Transform targetSpawnTransform;
    public void Initialize(MapSubSectionController controller)
    {
        this.controller = controller;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            controller.StartTransitionSection(targetSpawnTransform).Forget();
        }
    }
}
