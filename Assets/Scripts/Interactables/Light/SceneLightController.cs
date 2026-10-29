using System;
using UnityEngine;

[Serializable]
public class GroupSceneLightPhase
{
    public GroupSceneLight[] groupSceneLightArr;
}
[Serializable]
public class GroupSceneLight
{
    public BaseLight[] sceneLightArr;
    public SectionType roomType;
}
public class SceneLightController : MonoBehaviour
{
    //[SerializeField] private BaseLight[] sceneLightArr;
    private GamePhaseController gamePhaseController => GameController.instance.gamePhaseController;
    private MapSubSectionController subSectionController => GameController.instance.mapSubSectionController;

    [SerializeField] private GroupSceneLightPhase[] sceneLightPhaseArr;
    [SerializeField] private BaseHideable[] hideableArr;
    private void Start()
    {
        foreach(BaseHideable hideable in hideableArr)
        {
            hideable.Initialize(this);
        }

        InitializeSectionLight();
    }
    public void InitializeSectionLight()
    {
        foreach (GroupSceneLight subGroup in sceneLightPhaseArr[gamePhaseController.GetPhase].groupSceneLightArr)
        {
            if (subGroup.roomType == subSectionController.GetSectionType)
            {
                foreach (BaseLight light in subGroup.sceneLightArr)
                {
                    light.DisableLight(true);
                    if (light.previouslyActive == true)
                    {
                        light.previouslyActive = false;
                        light.ToggleLight(false);
                    }
                }
            }
            else
            {
                foreach (BaseLight light in subGroup.sceneLightArr)
                {
                    light.DisableLight(false);
                }
            }
        }
    }
    public void ToggleHidingLight(bool status)
    {
        if(status == false)
        {
            foreach (GroupSceneLight subGroup in sceneLightPhaseArr[gamePhaseController.GetPhase].groupSceneLightArr)
            {
                if (subGroup.roomType == subSectionController.GetSectionType)
                {
                    foreach (BaseLight light in subGroup.sceneLightArr)
                    {
                        if (light.active == true)
                        {
                            light.previouslyActive = true;
                            light.ToggleLight(false);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (GroupSceneLight subGroup in sceneLightPhaseArr[gamePhaseController.GetPhase].groupSceneLightArr)
            {
                if (subGroup.roomType == subSectionController.GetSectionType)
                {
                    foreach (BaseLight light in subGroup.sceneLightArr)
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
    }
}
