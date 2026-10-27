using System;
using UnityEngine;

public abstract class BaseMonster<TState> : StateManager<TState> where TState : Enum
{
    protected string playerTag = "Player";
    protected string lightTag = "Light";
    [SerializeField] protected Rigidbody2D rb2d;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected bool isFacingRight;
    protected float delayStateTime;
    [SerializeField] protected float patrolSpeed;
}
