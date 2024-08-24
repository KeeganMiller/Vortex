using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class ResourceManager
{
    public Scene Owner { get; }

    private List<Element> _elements = new List<Element>();
    private bool _hasElementToStart = false;

    public ResourceManager(Scene owner)
    {
        Owner = owner;
    }

    public void Start()
    {

    }

    public void Update(float dt)
    {
        foreach(var element in _elements)
        {
            if (element.HasStarted && element.IsActive)
                element.Update(dt);
        }

        if(_hasElementToStart)
        {
            foreach(var element in _elements)
            {
                if (!element.HasStarted && element.IsActive)
                    element.Start();
            }
        }
    }

    public void Draw()
    {
        foreach(var element in _elements)
            if(element.HasStarted && element.IsActive)
                element.Draw();
    }

    public void Stop()
    {

    }

    public bool AddElement(Element element)
    {
        if(element != null && !_elements.Contains(element)) 
        {
            _elements.Add(element);
            element.Initialize(this);
            _hasElementToStart = true;
            return true;
        }

        return false;
    }

    public bool RemoveElement(Element element)
    {
        if(element != null && _elements.Contains(element))
        {
            _elements.Remove(element);
            return true;
        }

        return false;
    }
}