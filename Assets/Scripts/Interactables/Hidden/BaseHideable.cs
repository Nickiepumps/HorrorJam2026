using UnityEngine;
using UnityEngine.Rendering.Universal;
public class BaseHideable : MonoBehaviour
{
    private SceneLightController lightController;
    [SerializeField] private Animator animatorHideable;
    [SerializeField] private Light2D lightHiddenView;
    public void Initialize(SceneLightController controller)
    {
        lightController = controller;
    }
    public virtual void Hide()
    {
        lightController.ToggleHidingLight(false);
        lightHiddenView.enabled = true;
    }
    public virtual void Unhide()
    {
        lightController.ToggleHidingLight(true);
        lightHiddenView.enabled = false;
    }
}
