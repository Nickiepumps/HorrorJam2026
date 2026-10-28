using UnityEngine;

public class SceneLightController : MonoBehaviour
{
    [SerializeField] private BaseLight[] sceneLightArr;
    [SerializeField] private BaseHideable[] hideableArr;
    private void Start()
    {
        foreach(BaseHideable hideable in hideableArr)
        {
            hideable.Initialize(this);
        }
    }
    public void ToggleHidingLight(bool status)
    {
        if(status == false)
        {
            foreach (BaseLight light in sceneLightArr)
            {
                if (light.active == true)
                {
                    light.previouslyActive = true;
                    light.ToggleLight(false);
                }
            }
        }
        else
        {
            foreach (BaseLight light in sceneLightArr)
            {
                if (light.previouslyActive == true)
                {
                    light.previouslyActive = false;
                    light.ToggleLight(true);
                }
            }
        }
    }
}
