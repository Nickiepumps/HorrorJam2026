using System.Collections.Generic;
using UnityEngine;

//assign only sections that enemy can follow player/ patrol around 
public enum HouseSection
{
    None, Floor2, Floor3,
}

public class EnemyWaypointController : MonoBehaviour
{
    [SerializeField] private DadMonster dadMonster;
    [SerializeField] private MomMonster momMonster;
    [SerializeField] private BrotherMonster broMonster;

    [SerializeField] private Transform[] floor2PatrolWaypointsArr = new Transform[2];
    [SerializeField] private Transform[] floor3PatrolWaypointsArr = new Transform[2];
    private Dictionary<HouseSection, Transform[]> sectionWaypointsDict;

    private void Start()
    {
        SetupSectionWaypointDict();
        momMonster.Initialize(this);
    }

    private void SetupSectionWaypointDict()
    {
        sectionWaypointsDict = new()
        {
            { HouseSection.Floor2, floor2PatrolWaypointsArr},
            { HouseSection.Floor3, floor3PatrolWaypointsArr},
        };
    }

    public void AssignWaypoints<TState>(BaseMonster<TState> monster, HouseSection section) where TState : System.Enum
    {
        if(sectionWaypointsDict.TryGetValue(section, out Transform[] newWaypintsArr))
        {
            monster.AssignNewWaypoints(newWaypintsArr);
        }       
    }

}
