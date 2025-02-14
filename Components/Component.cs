﻿using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public class Component
{
    public string ComponentId { get; set; }
    public string Name;
    public Element Owner { get; private set; }
    public bool IsSoloComponent { get; set; } = false;


    // == Active Status == //
    public bool HasStarted { get; private set; } = false;
    public bool HasInitialized { get; private set; } = false;
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
        if(Owner != null && Owner.Owner != null && !Owner.Owner.AllResourcesLoaded)
            Owner.Owner.OnFinishLoadResources += Constructor;

        HasInitialized = true;
    }

    public virtual void Constructor(ResourceManager resourceManager)
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

    public static Component? FindComponentById(string componentId)
    {
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var resource in resources)
        {
            foreach(var element in resource.GetAllElements())
            {
                var comp = element.GetComponent(componentId);
                if(comp != null)
                    return comp;
            }
        }

        return null;
    }

    public static T? FindComponentOfType<T>() where T : Component
    {
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var resource in resources)
        {
            foreach(var element in resource.GetAllElements())
            {
                var comp = element.GetComponent<T>();
                if(comp != null)
                    return comp;
            }
        }

        return null;
    }

    public static List<T> FindAllComponentsOfType<T>() where T : Component
    {
        var comps = new List<T>();
        var resources = SceneManager.GetAllResourceManagers();
        foreach(var resource in resources)
        {
            foreach(var element in resource.GetAllElements())
            {
                var comp = element.GetComponent<T>();
                if(comp != null)
                    comps.Add(comp);
            }
        }

        return comps;
    }
}