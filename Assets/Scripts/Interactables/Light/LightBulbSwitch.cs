using UnityEngine;
public class LightBulbSwitch : MonoBehaviour
{
    private bool isSwitchOn = false;
    [SerializeField] private BaseLight lightBulb; 
    public void ToggleSwitch()
    {
        lightBulb.ToggleLight(!lightBulb.active);
    }
}
