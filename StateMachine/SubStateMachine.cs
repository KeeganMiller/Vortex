using System.Collections.Generic;

namespace Vortex;

public abstract class SubStateMachine : State
{

    protected State _currentState;                      // Reference to the state we are currently updating
    public State EntryState;                            // State that will run when the state machine starts

    private List<StateTransition> _possibleTransitions = new List<StateTransition>();
    protected bool _isTransitioning = false;

    protected List<StateUpdater> Updaters = new List<StateUpdater>();

    public SubStateMachine(StateMachine owner, bool hasExit = false, string stateName = "") : base(owner, hasExit, stateName)
    {
    }

    public SubStateMachine(StateMachine owner, SubStateMachine subState, bool hasExit = false, string stateName = "") : base(owner, subState, hasExit, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CreateStates();
        if(EntryState != null)
            ForceSetState(EntryState);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(_currentState != null)
            _currentState.Update(dt);

        HandleUpdaters(dt);

        if(!_isTransitioning)
            CheckForTransition();
            
    }

    private void CheckForTransition()
    {
        foreach(var trans in _possibleTransitions)
        {
            if(trans.CanTransition(Owner.Properties))
            {
                _isTransitioning = true;
                SetState(trans.NextState);
                return;
            }
        }
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
        {
            _currentState.Enter();
            _possibleTransitions = _currentState.Transitions;
        }
            

        _isTransitioning = false;
    }

    /// <summary>
    /// Handles updating of active state, and also toggling states active
    /// </summary>
    /// <param name="dt">Delta Time</param>
    private void HandleUpdaters(float dt)
    {
        foreach(var updater in Updaters)
        {
            if(updater.IsActive)
                updater.Update(dt);

            if(updater.StateRef == _currentState)
            {
                if(!updater.IsActive)
                    updater.IsActive = true;
            } else
            {
                if(updater.IsActive)
                    updater.IsActive = false;
            }
        }
    }

    protected abstract void CreateStates();
}