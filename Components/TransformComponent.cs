using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class TransformComponent : Component
{
    private Vector2 _localPosition;
    public Vector2 Position
    {
        get 
        {
            if(Owner.IsCameraRelated)
            {
                return Game.GetPositionBasedOnCamera(Owner.Parent.Transform.Position + _localPosition);
            } else 
            {
                return Owner.Parent != null ? Owner.Parent.Transform.Position + _localPosition : _localPosition;
            }
        }
        set => _localPosition = value;
    }

    private Vector2 _localScale = Vector2.One;
    public Vector2 Scale
    {
        get => Owner.Parent != null ? Owner.Parent.Transform.Scale * _localScale : _localScale;
        set => _localScale = value;
    }

    private float _localRotation = 0;
    public float Rotation
    {
        get => Owner.Parent != null ? Owner.Parent.Transform.Rotation + _localRotation : _localRotation;
        set => _localRotation = value;
    }
}
