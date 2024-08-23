using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class Element
{
    public string ObjectId { get; } = Guid.NewGuid().ToString();
    public string Name;

    // == Element Status == //
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

    public Element(string name = "Element")
    {
        this.Name = name;
    }

    public virtual void Initialize()
    {

    }

    public virtual void Start()
    {

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