using System.Collections.Generic;
using Microsoft.VisualBasic;
using Raylib_cs;

namespace Vortex;

public class Element
{
    public string ObjectId { get; set; }
    public string Name;                         // Reference to the name of the element
    public ResourceManager Owner { get; private set; }                  // reference to the resource manager that owns this
    public List<string> Tags = new List<string>();                          // List of all the tags assigned to this object

    public bool IsCameraRelated = false;                            // If this is camera relative

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
                {
                    Enable?.Invoke();
                    foreach(var child in _children)
                        child.IsActive = true;

                    foreach(var comp in _components)
                        comp.IsActive = true;
                }
                else
                {
                    Disable?.Invoke();
                    foreach(var child in _children)
                        child.IsActive = false;

                    foreach(var comp in _components)
                        comp.IsActive = false;
                }
            }
        }
    }

    public System.Action Enable;
    public System.Action Disable;

    public bool HasStarted { get; private set; } = false;                       // Flag if the element has started

    // == Parenting == //
    public Element Parent { get; protected set; }                           // Reference to the parent of this element
    public string ElementParentId { get; set; }
    protected List<Element> _children = new List<Element>();                    // List of all the children for this element

    // == Components == //
    private List<Component> _components = new List<Component>();                // List of all components on this element
    private bool _hasComponentToStart = false;
    public TransformComponent Transform { get; private set; }                            // Reference to the transform component

    public Element(string name = "Element")
    {
        this.Name = name;
    }

    public void SetTransform(TransformComponent comp)
    {
        Transform = comp;
        AddComponent(comp);
    }

    /// <summary>
    /// Called when the element has been added to the resource manager
    /// </summary>
    /// <param name="owner">Reference to the resource manager</param>
    public virtual void Initialize(ResourceManager owner)
    {
        Owner = owner;
        if(Name == "SaveBtnText")
            Debug.Print("SaveBtn Initizled", EPrintMessageType.PRINT_Log);

        if(owner != null)
            owner.OnFinishLoadResources += LoadRequiredProperties;
    }

    /// <summary>
    /// Use to handle any assets that need to be loaded from file
    /// </summary>
    public virtual void LoadRequiredProperties()
    {
        FindParent();
    }

    /// <summary>
    /// Called in the first frame this element is active
    /// </summary>
    public virtual void Start()
    {
        if(Name == "SaveBtnText")
            Debug.Print("SaveBtn Started", EPrintMessageType.PRINT_Log);

        
        HasStarted = true;
    }

    /// <summary>
    /// Called each frame
    /// </summary>
    /// <param name="dt"></param>
    public virtual void Update(float dt)
    {
        UpdateComponents(dt);
    }

    public virtual void Draw()
    {
        DrawComponents();
    }

    /// <summary>
    /// Called to destroy the element
    /// </summary>
    public virtual void Destroy()
    {
        if(Owner != null)
            Owner.RemoveElement(this);
    }

    /// <summary>
    /// Sets the parent for this element
    /// </summary>
    /// <param name="parent">Parent to set</param>
    public void SetParent(Element parent)
    {
        // Remove from the current parent
        if (Parent != null)
            Parent.RemoveChild(this);

        Parent = parent;                    // Set the parent
        // Add this element to the parent
        if (Parent != null)
            Parent.AddChild(this);
    }

    /// <summary>
    /// Adds a child element
    /// </summary>
    /// <param name="child">Child Element</param>
    public void AddChild(Element child)
    {
        if(!_children.Contains(child))
            _children.Add(child);
    }

    /// <summary>
    /// Removes a child element
    /// </summary>
    /// <param name="child">Child Element</param>
    public void RemoveChild(Element child)
    {
        if(_children.Contains(child))
            _children.Remove(child);
    }

    /// <summary>
    /// Gets the child at index
    /// </summary>
    /// <param name="index">Index of the child</param>
    /// <returns>Element that is a child at index</returns>
    public Element GetChild(int index)
    {
        if(index < _children.Count)
        {
            return _children[index];
        }

        return null;
    }

    /// <summary>
    /// Gets a child based on the element name
    /// </summary>
    /// <param name="name">Name of the element</param>
    /// <returns>Child element with name</returns>
    public Element GetChild(string name)
    {
        foreach(var e in _children)
            if(e.Name == name)
                return e;

        return null;
    }

    /// <summary>
    /// Gets a child based on the tag
    /// </summary>
    /// <param name="tag">Tag to find on child</param>
    /// <returns>Child with specified tag</returns>
    public Element GetChildByTag(string tag)
    {
        foreach(var e in _children)
            if(e.HasTag(tag))
                return e;

        return null;
    }

    /// <summary>
    /// Gets all the children of this element
    /// </summary>
    /// <returns></returns>
    public List<Element> GetChildren() => _children;

    // == Component Methods == //
    /// <summary>
    /// Adds a new component to this element
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="component">Component</param>
    /// <returns>If the component was added</returns>
    public bool AddComponent<T>(T component) where T : Component
    {
        // Check that the component isn't already attached
        foreach(var comp in _components)
        {
            if (comp is T)
                return false;
        }

        _components.Add(component);                 // Add the component
        component.Initialize(this);                     // Initialzie the component     
        return true;                    // return that the component was added
    }

    /// <summary>
    /// Removes a component from this element
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <returns>If the component was removed</returns>
    public bool RemoveComponent<T>() where T : Component
    {
        // Find the component and destroy it
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

    /// <summary>
    /// Gets a component attached to this element
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <returns>Component</returns>
    public T GetComponent<T>() where T : Component
    {
        foreach(var comp in _components)
        {
            if (comp is T compT)
                return compT;
        }

        return null;
    }

    /// <summary>
    /// Finds and returns the specified component from within a child
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <returns>Reference to the component</returns>
    public T? GetComponentInChild<T>() where T : Component
    {
        foreach(var child in _children)
        {
            var comp = child.GetComponent<T>();
            if(comp != null)
                return comp;
        }

        return null;
    }

    /// <summary>
    /// Finds and returns the component with specified ID
    /// </summary>
    /// <param name="componentId"></param>
    /// <returns></returns>
    public Component? GetComponent(string componentId)
    {
        foreach(var comp in _components)
            if(comp.ComponentId == componentId)
                return comp;

        return null;
    }

    /// <summary>
    /// Finds and returns the component in a child of this element with the specified component id
    /// </summary>
    /// <typeparam name="T">Component Type</typeparam>
    /// <param name="componentId">Component ID</param>
    /// <returns>Component of type</returns>
    public T? GetComponentInChild<T>(string componentId) where T : Component
    {
        foreach(var child in _children)
        {
            var component = child.GetComponent(componentId);
            if(component != null && component is T compT)
                return compT;
        }

        return null;
    }
    

    /// <summary>
    /// Handles updating the component
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateComponents(float dt)
    {
        // Update the active components that have already started
        foreach(var comp in _components.ToList())
        {
            if (comp.HasStarted && comp.IsActive)
                comp.Update(dt);
        }

        // Start any components that haven't started & are active
        foreach(var comp in _components.ToList())
        {
            if (!comp.HasStarted && comp.IsActive)
            {
                comp.Start();
                _hasComponentToStart = false;
            }
        }
        
    }
    
    /// <summary>
    /// Checks if the tag is assigned to this element
    /// </summary>
    /// <param name="tag">Tag to find</param>
    /// <returns>If the tag is assigned</returns>
    public bool HasTag(string tag)
    {
        foreach(var t in Tags)
            if(t == tag)
                return true;

        return false;
    }

    /// <summary>
    /// Handles drawing the components besides UI components & Sprite components
    /// </summary>
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

    public void FindParent()
    {
        if(Parent != null)
            return;

        if(!string.IsNullOrEmpty(ElementParentId))
        {
            var parent = Owner.GetElementById(ElementParentId);
            if(parent != null)
                SetParent(parent);
            else
                Debug.Print($"{Name}::Start (Element) -> Failed to get reference to the parent", EPrintMessageType.PRINT_Log);
        }
    }

    public static List<Element> GetAllElementsWithName(string name, bool onlyActive = false)
    {
        var resources = SceneManager.GetAllResourceManagers();
        var foundElements = new List<Element>();
        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements().ToList())
            {
                if(onlyActive)
                {
                    if(e.IsActive && e.Name == name)
                        foundElements.Add(e);
                } else 
                {
                    if(e.Name == name)
                        foundElements.Add(e);
                }
            }
        }

        return foundElements;
    }

    public static Element GetFirstElementWithName(string name, bool onlyActive = false)
    {
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements())
            {
                if(onlyActive)
                {
                    if(e.IsActive && e.Name == name)
                        return e;
                } else 
                {
                    if(e.Name == name)
                        return e;
                }
            }
        }

        return null;
    }

    public static List<Element> GetAllElementsWithTag(string tag, bool onlyActive = false)
    {
        var resources = SceneManager.GetAllResourceManagers();
        var foundElemenets = new List<Element>();
        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements())
            {
                if(onlyActive)
                {
                    if(e.IsActive && e.HasTag(tag))
                        foundElemenets.Add(e);
                } else 
                {
                    if(e.HasTag(tag))
                        foundElemenets.Add(e);
                }
            }
        }

        return foundElemenets;
    }

    public static Element GetFirstElementWithTag(string tag, bool onlyActive = false)
    {
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements())
            {
                if(onlyActive)
                {
                    if(e.IsActive && e.HasTag(tag))
                        return e;
                } else 
                {
                    if(e.HasTag(tag))
                        return e;
                }
            }
        }

        return null;
    }

    public static List<Element> GetAllElementsWithComponent<T>(bool onlyActive = false) where T : Component
    {
        var resources = SceneManager.GetAllResourceManagers();
        var foundElements = new List<Element>();

        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements())
            {
                if(onlyActive)
                {
                    if(e.IsActive)
                    {
                        var comp = e.GetComponent<T>();
                        if(comp != null)
                            foundElements.Add(e);
                    }
                } else 
                {
                    var comp = e.GetComponent<T>();
                    if(comp != null)
                        foundElements.Add(e);
                }
            }
        }

        return foundElements;
    }

    public static Element GetFirstElementWithComponent<T>(bool onlyActive = false) where T : Component
    {
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var r in resources)
        {
            foreach(var e in r.GetAllElements())
            {
                if(onlyActive)
                {
                    if(e.IsActive)
                    {
                        var comp = e.GetComponent<T>();
                        if(comp != null)
                            return e;
                    }
                } else 
                {
                    var comp = e.GetComponent<T>();
                    if(comp != null)
                        return e;
                }
            }
        }

        return null;
    }
}