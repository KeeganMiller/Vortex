using System.Collections.Generic;

namespace Vortex;

public class State
{
    public string StateName = "";
    public StateMachine Owner { get; private set; }                     // Reference to the state machine that owns this
    public SubStateMachine SubState { get; private set; }


    public State ExitState;
    public bool HasExit;
    public bool ClearExitStateOnExit = false;

    public List<StateTransition> Transitions = new List<StateTransition>();

    public State(StateMachine owner, bool hasExit = false, string stateName = "")
    {
        Owner = owner;
        HasExit = hasExit;
        StateName = stateName;
    }

    public State(StateMachine owner, SubStateMachine subState, bool hasExit = false, string stateName = "")
    {
        Owner = owner;
        SubState = subState;
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
        if(ExitState != null)
        {
            if(SubState != null)
            {
                SubState.SetState(ExitState, true);
                if(ClearExitStateOnExit)
                    ExitState = null;
            } else 
            {
                if(Owner != null)
                {
                    Owner.SetState(ExitState, true);
                    if(ClearExitStateOnExit)
                        ExitState = null;
                }
            }
        }
    }

    public virtual void Exit()
    {

    }

}