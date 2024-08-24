using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class Component
{
    public string ComponentId { get; } = Guid.NewGuid().ToString();
    public string Name;
    public Element Owner { get; private set; }


    // == Active Status == //
    public bool HasStarted { get; private set; } = false;
    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if(value != _isActive)
            {
                _isActive = value;
                if (_isActive)
                    Enable?.Invoke();
                else
                    Disable?.Invoke();
            }
        }
    }

    public System.Action Enable;
    public System.Action Disable;

    public Component(string name = "Component")
    {
        Name = name;
    }

    public virtual void Initialize(Element owner)
    {
        Owner = owner;
    }

    public virtual void Start()
    {
        HasStarted = true;
    }

    public virtual void Update(float dt)
    {

    }

    public virtual void Draw()
    {

    }

    public virtual void Destroy()
    {

    }
}