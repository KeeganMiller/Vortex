using System.Collections.Generic;

namespace Vortex;

public abstract class StateMachine : Component
{
    protected State _currentState;                      // Reference to the state we are currently updating
    public State EntryState;                            // State that will run when the state machine starts

    public override void Start()
    {
        base.Start();

    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(_currentState != null)
            _currentState.Update(dt);
    }

    /// <summary>
    /// Sets the next state
    /// </summary>
    /// <param name="state">Next State</param>
    /// <param name="isExit">If this is called from and exit method</param>
    public void SetState(State state, bool isExit = false)
    {
        if(isExit || !_currentState.HasExit)
        {
            ForceSetState(state);
            return;
        }

        _currentState.ExitState = state;
        _currentState.ClearExitStateOnExit = true;
    }

    /// <summary>
    /// Force sets the current state without any exit 
    /// (Mainly used by SetState())
    /// </summary>
    /// <param name="state">State to set as current</param>
    public void ForceSetState(State state)
    {
        if(_currentState != null)
            _currentState.Exit();

        _currentState = state;
        if(_currentState != null)
            _currentState.Enter();
    }
}