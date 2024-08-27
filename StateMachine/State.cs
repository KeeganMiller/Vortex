using System.Collections.Generic;

namespace Vortex;

public class State
{
    public string StateName = "";
    public StateMachine Owner { get; private set; }                     // Reference to the state machine that owns this


    public State ExitState;
    public bool HasExit;
    public bool ClearExitStateOnExit = false;

    public State(StateMachine owner, bool hasExit = false, string stateName = "")
    {
        Owner = owner;
        HasExit = hasExit;
        StateName = stateName;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update(float dt)
    {

    }

    protected virtual void OnFinish()
    {

    }

    public virtual void Exit()
    {
        
    }

}