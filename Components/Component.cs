using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class Component
{
    public string ComponentId { get; } = Guid.NewGuid().ToString();
    public string Name;
    public Element Owner { get; private set; }

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