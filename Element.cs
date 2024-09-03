using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Raylib_cs;
using Vortex.UI;

namespace Vortex;

public class Element
{
    public string ObjectId { get; } = Guid.NewGuid().ToString();
    public string Name;
    public ResourceManager Owner { get; private set; }

    public bool IsCameraRelated = false;

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
    private bool _hasComponentToStart = false;
    public TransformComponent Transform { get; }

    public Element(string name = "Element")
    {
        this.Name = name;
        Transform = new TransformComponent();
        AddComponent<TransformComponent>(Transform);
    }

    public virtual void Initialize(ResourceManager owner)
    {
        Owner = owner;
    }

    public virtual void Start()
    {
        HasStarted = true;
    }

    public virtual void Update(float dt)
    {
        UpdateComponents(dt);
    }

    public virtual void Draw()
    {
        DrawComponents();
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

    public Element GetChild(int index)
    {
        if(index < _children.Count)
        {
            return _children[index];
        }

        return null;
    }

    public Element GetChild(string name)
    {
        foreach(var e in _children)
            if(e.Name == name)
                return e;

        return null;
    }

    public List<Element> GetChildren() => _children;

    // == Component Methods == //
    public bool AddComponent<T>(T component) where T : Component
    {
        foreach(var comp in _components)
        {
            if (comp is T)
                return false;
        }

        _components.Add(component);
        component.Initialize(this);
        _hasComponentToStart = true;
        return true;
    }

    public bool RemoveComponent<T>() where T : Component
    {
        foreach(var comp in _components)
        {
            if(comp is T)
            {
                _components.Remove(comp);
                comp.Destroy();
                return true;
            }
        }

        return false;
    }

    public T GetComponent<T>() where T : Component
    {
        foreach(var comp in _components)
        {
            if (comp is T compT)
                return compT;
        }

        return null;
    }

    private void UpdateComponents(float dt)
    {
        foreach(var comp in _components.ToList())
        {
            if (comp.HasStarted && comp.IsActive)
                comp.Update(dt);
        }


        foreach(var comp in _components.ToList())
        {
            if (!comp.HasStarted && comp.IsActive)
            {
                comp.Start();
                _hasComponentToStart = false;
            }
        }
        
    }

    private void DrawComponents()
    {
        foreach (var comp in _components)
        {
            if (comp.HasStarted && comp.IsActive)
            {
                if(comp is not SpriteComponent && comp is not UIComponent)
                    comp.Draw();
            }
        }
    }
}