using System;
using UnityEngine;

public abstract class BaseMonster<TState> : StateManager<TState> where TState : Enum
{
    protected string playerTag = "Player";
    protected string lightTag = "Light";
    [SerializeField] protected Rigidbody2D rb2d;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected EnemyWaypointController enemyWaypointController => GameController.instance.enemyWaypointController;
    protected bool isFacingRight;
    protected float delayStateTime = 1;
    [SerializeField] protected float patrolSpeed;

    [SerializeField] protected SectionType startSection;

    public virtual void Initialize()
    {
        enemyWaypointController.AssignWaypoints(this, startSection);
        SetupStateHandler();
        OnEnterState(CurrentState);
    }

    protected virtual void Start()
    {
        Initialize();
    }



    public virtual void AssignNewWaypoints(Transform[] newWaypointsArr)
    {

    }
}
