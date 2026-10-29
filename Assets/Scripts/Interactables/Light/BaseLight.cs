using UnityEngine;
using UnityEngine.Rendering.Universal;
public class BaseLight : MonoBehaviour
{
    [SerializeField] private Light2D lightData;
    [SerializeField] private AudioSource audioLightHum;
    public float intensity => lightData.intensity;
    public Color lightColor => lightData.color;
    public bool active => lightData.enabled;
    public bool previouslyActive;
    public void ToggleLight(bool state)
    {
        lightData.enabled = state;
        if(audioLightHum != null)
        {
            if(state == true)
            {
                audioLightHum.Play();
            }
            else
            {
                audioLightHum.Stop();
            }
        }
    }
    public void DisableLight(bool state)
    {
        lightData.gameObject.SetActive(state);
        if (audioLightHum != null)
        {
            if (state == true)
            {
                audioLightHum.Play();
            }
            else
            {
                audioLightHum.Stop();
            }
        }
    }
}
