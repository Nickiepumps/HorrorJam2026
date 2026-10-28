using UnityEngine;
using UnityEngine.Rendering.Universal;
public class BaseLight : MonoBehaviour
{
    [SerializeField] private Light2D lightData;
    public float intensity => lightData.intensity;
    public Color lightColor => lightData.color;
    public bool active => lightData.enabled;
    public bool previouslyActive;
    public void ToggleLight(bool state)
    {
        lightData.enabled = state;
    }
}
