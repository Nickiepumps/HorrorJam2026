using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBulbSwitch : MonoBehaviour
{
    private bool isSwitchOn = false;
    [SerializeField] private Light2D lightBulb;
    [SerializeField] private float maxIntensity;

    private void Start()
    {
        if(lightBulb.intensity > 0)
        {
            isSwitchOn = true;
        }
    }

    public void ToggleSwitch()
    {
        isSwitchOn = !isSwitchOn;
        if (isSwitchOn)
        {
            lightBulb.intensity = maxIntensity;
        }
        else
        {
            lightBulb.intensity = 0;
        }
        print($"light switch turned on: {isSwitchOn}");
    }
}
