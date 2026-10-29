using System.Collections.Generic;
using UnityEngine;


public class EnemyWaypointController : MonoBehaviour
{
    [SerializeField] private DadMonster dadMonster;
    [SerializeField] private MomMonster momMonster;
    [SerializeField] private BrotherMonster broMonster;

    [SerializeField] private Transform[] floor2PatrolWaypointsArr = new Transform[2];
    [SerializeField] private Transform[] floor3PatrolWaypointsArr = new Transform[2];
    private Dictionary<SectionType, Transform[]> sectionWaypointsDict;

    private void Start()
    {
        SetupSectionWaypointDict();
        momMonster.Initialize(this);
    }

    private void SetupSectionWaypointDict()
    {
        sectionWaypointsDict = new()
        {
            { SectionType.Floor2, floor2PatrolWaypointsArr},
            { SectionType.Floor3, floor3PatrolWaypointsArr},
        };
    }

    public void AssignWaypoints<TState>(BaseMonster<TState> monster, SectionType section) where TState : System.Enum
    {
        if(sectionWaypointsDict.TryGetValue(section, out Transform[] newWaypintsArr))
        {
            monster.AssignNewWaypoints(newWaypintsArr);
        }       
    }

}
