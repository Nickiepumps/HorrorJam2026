using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<TState> : MonoBehaviour where TState : Enum
{
    [Header("State")]
    [SerializeField] private TState initialState;
    public TState CurrentState { get; private set; }
    public TState PreviousState { get; private set; }
    protected Dictionary<TState, Action> stateEnterHandlers;

    protected virtual void Awake()
    {
        CurrentState = initialState;
        PreviousState = initialState;
    }

    protected virtual void Update()
    {
        OnUpdateState(CurrentState);
    }
        
    public bool ChangeState(TState newState)
    {
        TState oldState = CurrentState;

        if (Equals(oldState, newState))
            return false;

        PreviousState = oldState;
        CurrentState = newState;

        OnEnterState(newState);

        return true;
    }

    protected virtual void OnEnterState(TState newState) 
    {
        if (stateEnterHandlers != null && stateEnterHandlers.TryGetValue(newState, out var handler))
        {
            handler.Invoke();
        }
    }
    protected virtual void OnUpdateState(TState newState) { }
    protected abstract void SetupStateHandler();
}