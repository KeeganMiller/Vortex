using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Raylib_cs;

namespace Vortex;

public enum EAnchorLocation
{
    ANCHOR_None,
    ANCHOR_TopLeft,
    ANCHOR_TopCenter,
    ANCHOR_TopRight,
    ANCHOR_MiddleLeft,
    ANCHOR_MiddleCenter,
    ANCHOR_MiddleRight,
    ANCHOR_BottomLeft,
    ANCHOR_BottomCenter,
    ANCHOR_BottomRight
}

public enum EOriginLocation
{
    ORIGIN_None,
    ORIGIN_TopLeft,
    ORIGIN_TopCenter,
    ORIGIN_TopRight,
    ORIGIN_MiddleLeft,
    ORIGIN_MiddleCenter,
    ORIGIN_MiddleRight,
    ORIGIN_BottomLeft,
    ORIGIN_BottomCenter,
    ORIGIN_BottomRight
}

public class UIComponent : Component
{
    public EAnchorLocation Anchor { get; protected set; }
    public EOriginLocation Origin { get; protected set; }
    protected TransformComponent _ownerTransform;

    public float Width;
    public float Height;

    public override void Start()
    {
        base.Start();
        if(Owner != null)
            _ownerTransform = Owner.Transform;

        if(Owner != null)
            Owner.IsCameraRelated = false;
    }

    public void SetAnchor(EAnchorLocation anchor)
    {
        Anchor = anchor;
        if(_ownerTransform == null)
            return;
        
        switch(Anchor)
        {
            case EAnchorLocation.ANCHOR_TopLeft:
                _ownerTransform.Position = Owner.Parent == null ? Vector2.Zero + GetOrigin() : Owner.Parent.Transform.Position + GetOrigin();
                break;
            case EAnchorLocation.ANCHOR_TopCenter:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, 0) + GetOrigin();
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width / 2, 9) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_TopRight:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, 0) + GetOrigin();
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width, 0) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_MiddleLeft:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(0, Game.WindowSettings.WindowHeight / 2) + GetOrigin();
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(0, parentComp.Height / 2) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_MiddleCenter:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, Game.WindowSettings.WindowHeight / 2) + GetOrigin();
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width / 2, parentComp.Height / 2) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_MiddleRight:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight / 2) + GetOrigin();
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width, parentComp.Height / 2) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_BottomLeft:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(0, Game.WindowSettings.WindowHeight) + GetOrigin();
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(0, parentComp.Height) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_BottomCenter:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, Game.WindowSettings.WindowHeight) + GetOrigin();
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width / 2, parentComp.Height) + GetOrigin();
                }
                break;
            case EAnchorLocation.ANCHOR_BottomRight:
                if(Owner.Parent == null)
                {
                    _ownerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight) + GetOrigin();
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        _ownerTransform.Position = new Vector2(parentComp.Width, parentComp.Height) + GetOrigin();
                }
                break;
        }
    }

    public void SetOrigin(EOriginLocation origin)
    {
        Origin = origin;
        
    }

    public void SetOriginAndAnchor(EOriginLocation origin, EAnchorLocation anchor)
    {
        SetOrigin(origin);
        SetAnchor(anchor);
    }

    public virtual Vector2 GetOrigin()
    {
        switch(Origin)
        {
            case EOriginLocation.ORIGIN_TopCenter:
                return new Vector2(Width / 2, 0);
            case EOriginLocation.ORIGIN_TopRight:
                return new Vector2(Width, 0);
            case EOriginLocation.ORIGIN_MiddleLeft:
                return new Vector2(0, Height / 2);
            case EOriginLocation.ORIGIN_MiddleCenter:
                return new Vector2(Width / 2, Height / 2);
            case EOriginLocation.ORIGIN_MiddleRight:
                return new Vector2(Width, Height / 2);
            case EOriginLocation.ORIGIN_BottomLeft:
                return new Vector2(0, Height);
            case EOriginLocation.ORIGIN_BottomCenter:
                return new Vector2(Width / 2, Height);
            case EOriginLocation.ORIGIN_BottomRight:
                return new Vector2(Width, Height);
            default:
                return Vector2.Zero;
        }
    }

    public UIComponent GetParentUIComp()
    {
        return Owner.Parent.GetComponent<UIComponent>();
    }
}