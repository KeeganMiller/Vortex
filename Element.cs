using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class Element
{
    public string ObjectId { get; } = Guid.NewGuid().ToString();
    public string Name;

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