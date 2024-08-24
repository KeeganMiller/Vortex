﻿using System.Collections.Generic;
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

    public bool HasStarted { get; private set; } = false;

    // == Parenting == //
    public Element Parent { get; protected set; }
    protected List<Element> _children;

    // == Components == //
    private List<Component> _components = new List<Component>();

    public Element(string name = "Element")
    {
        this.Name = name;
    }

    public virtual void Initialize()
    {

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

    public void SetParent(Element parent)
    {
        if (Parent != null)
            Parent.RemoveChild(this);

        Parent = parent;
        if (Parent != null)
            Parent.AddChild(this);
    }

    public void AddChild(Element child)
    {
        if(!_children.Contains(child))
            _children.Add(child);
    }

    public void RemoveChild(Element child)
    {
        if(_children.Contains(child))
            _children.Remove(child);
    }
}