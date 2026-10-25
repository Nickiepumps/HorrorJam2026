using UnityEngine;

public class LightSwitchDetector : MonoBehaviour
{
    [SerializeField] private string targetTag = "LightSwitch";
    private bool isNearLightSwitch = true;
    private LightBulbSwitch availableSwitch;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && availableSwitch != null)
        {
            availableSwitch.ToggleSwitch();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && other.TryGetComponent<LightBulbSwitch>(out LightBulbSwitch lightSwitch))
        {
            availableSwitch = lightSwitch;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            availableSwitch = null;
        }
    }
}
