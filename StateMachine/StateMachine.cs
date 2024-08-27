using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Vortex;

public abstract class StateMachine : Component
{
    protected State _currentState;                      // Reference to the state we are currently updating
    public State EntryState;                            // State that will run when the state machine starts

    public List<StateProperty> Properties { get; private set;} = new List<StateProperty>();
    private List<StateTransition> _possibleTransitions = new List<StateTransition>();
    protected bool _isTransitioning = false;

    protected List<StateUpdater> Updaters = new List<StateUpdater>();

    public override void Start()
    {
        base.Start();
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
            if(trans.CanTransition(Properties))
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

    /// <summary>
    /// Sets/Creates Property value
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="key">Key for the property</param>
    /// <param name="value">New value of the property</param>
    /// <param name="propType">The type of property that is stored (only for new Properties)</param>
    public void SetPropertyValue<T>(string key, T value, EStatePropertyType propType = EStatePropertyType.PROP_None)
    {
        foreach(var prop in Properties)
        {
            if(prop.Key == key && prop is StateValue<T> propVal)
            {
                propVal.Value = value;
            }
        }

        Properties.Add(new StateValue<T>(key, value, propType));
    }

    /// <summary>
    /// Gets the value of a property
    /// </summary>
    /// <typeparam name="T">Type of property value</typeparam>
    /// <param name="key">Key for the property to find</param>
    /// <returns>Value of the state property</returns>
    public T GetPropertyValue<T>(string key)
    {
        foreach(var prop in Properties)
        {
            if(prop.Key == key && prop is StateValue<T> propVal)
                return propVal.Value;
        }
        
        Debug.Print($"StateMachine::GetPropertyValue -> Failed to find property: {key}", EPrintMessageType.PRINT_Warning);
        return default;
    }
}