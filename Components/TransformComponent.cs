using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class TransformComponent : Component
{
    private Vector2 _localPosition;
    public Vector2 LocalPosition => _localPosition;
    public Vector2 Position
    {
        get => Owner.Parent != null ? Owner.Parent.Transform.Position + _localPosition : _localPosition;
        set 
        {
            _localPosition = value;
            PositionUpdateEvent?.Invoke();
        }
    }
    
    public Action? PositionUpdateEvent;

    private Vector2 _localScale = Vector2.One;
    public Vector2 Scale
    {
        get => Owner.Parent != null ? Owner.Parent.Transform.Scale * _localScale : _localScale;
        set 
        {
            _localScale = value;
            ScaleUpdateEvent?.Invoke();
        }
    }

    public Action? ScaleUpdateEvent;

    private float _localRotation = 0;
    public float Rotation
    {
        get => Owner.Parent != null ? Owner.Parent.Transform.Rotation + _localRotation : _localRotation;
        set 
        {
            _localRotation = value;
            RotationUpdateEvent?.Invoke();
        }
    }

    public Action? RotationUpdateEvent;
}
