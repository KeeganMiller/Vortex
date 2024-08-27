using System.Collections.Generic;

namespace Vortex;

public class StateUpdater
{
    public State StateRef { get; private set; }
    public StateMachine Owner;
    public bool IsActive = false;

    public StateUpdater(State state, StateMachine owner)
    {
        StateRef = state;
        Owner = owner;
    }

    public virtual void Enter()
    {

    }

    public virtual void Update(float dt)
    {

    }

    public virtual void Exit()
    {

    }
}