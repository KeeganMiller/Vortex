using System.Collections.Generic;
using System.Numerics;
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
    public TransformComponent OwnerTransform {get; protected set;}

    public float Width;
    public float Height;
    public int ZIndex = 0;
    protected Vector2 _offset = Vector2.Zero;
    public Vector2 Offset
    {
        get => _offset;
        set 
        {
            _offset = value;
            SetAnchor(Anchor);
            SetOrigin(Origin);
        }
    }

    public bool DrawDebugRect = false;                      // Flag if to draw a debug to show bounds


    public bool IsMouseOver { get; private set; }
    public Action OnMouseEnter;
    public Action OnMouseExit;

    public override void Start()
    {
        base.Start();
        if(Owner != null)
            OwnerTransform = Owner.Transform;

        if(Owner != null)
            Owner.IsCameraRelated = false;

        SetAnchor(Anchor);
        SetOrigin(Origin);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        DetectMouseEnterAndExit();
    }

    public override void Draw()
    {
        base.Draw();
        if(Debug.DebugEnabled && this.DrawDebugRect)
            _DrawDebugRect_();
    }

    public void SetAnchor(EAnchorLocation anchor)
    {
        Anchor = anchor;
        if(OwnerTransform == null)
            return;

        
        switch(Anchor)
        {
            case EAnchorLocation.ANCHOR_TopLeft:
                OwnerTransform.Position = GetOrigin() + _offset;
                break;
            case EAnchorLocation.ANCHOR_TopCenter:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, 0) - GetOrigin() + _offset;
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width / 2, 0) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_TopRight:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, 0) - GetOrigin() + _offset;
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width, 0) - GetOrigin() + _offset;

                }
                break;
            case EAnchorLocation.ANCHOR_MiddleLeft:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(0, Game.WindowSettings.WindowHeight / 2) - GetOrigin() + _offset;
                }
                else
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(0, parentComp.Height / 2) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_MiddleCenter:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, Game.WindowSettings.WindowHeight / 2) - GetOrigin() + _offset;
                }
                else
                {        
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width / 2, parentComp.Height / 2) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_MiddleRight:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight / 2) - GetOrigin() + _offset;
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width, parentComp.Height / 2) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_BottomLeft:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(0, Game.WindowSettings.WindowHeight) - GetOrigin() + _offset;
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(0, parentComp.Height) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_BottomCenter:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth / 2, Game.WindowSettings.WindowHeight) - GetOrigin() + _offset;
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width / 2, parentComp.Height) - GetOrigin() + _offset;
                }
                break;
            case EAnchorLocation.ANCHOR_BottomRight:
                if(Owner.Parent == null)
                {
                    OwnerTransform.Position = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight) - GetOrigin() + _offset;
                } else 
                {
                    var parentComp = GetParentUIComp();
                    if(parentComp != null)
                        OwnerTransform.Position = new Vector2(parentComp.Width, parentComp.Height) - GetOrigin() + _offset;
                }
                break;
        }
    }

    public void SetOrigin(EOriginLocation origin)
    {
        Origin = origin;
        SetAnchor(Anchor);
    }

    public void SetOriginAndAnchor(EOriginLocation origin, EAnchorLocation anchor)
    {
        Origin = origin;
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

    public void DetectMouseEnterAndExit()
    {
        var mousePos = Input.GetMousePosition(false);
        if(mousePos.X >= (OwnerTransform.Position.X - GetOrigin().X) && mousePos.X < ((OwnerTransform.Position.X - GetOrigin().X) + this.Width))
        {
            if(mousePos.Y >= (OwnerTransform.Position.Y - GetOrigin().Y) && mousePos.Y < ((OwnerTransform.Position.Y - GetOrigin().Y) + this.Height))
            {
                if(!IsMouseOver)
                {
                    IsMouseOver = true;
                    OnMouseEnter?.Invoke();
                }
            } else 
            {
                if(IsMouseOver)
                {
                    IsMouseOver = false;
                    OnMouseExit?.Invoke();
                }
            }
        } else 
        {
            if(IsMouseOver)
            {
                IsMouseOver = false;
                OnMouseExit?.Invoke();
            }
        }
    }


    private void _DrawDebugRect_()
    {
        var rect = new Rectangle
        {
            X = OwnerTransform.Position.X,
            Y = OwnerTransform.Position.Y,
            Width = this.Width,
            Height = this.Height
        };

        Raylib.DrawRectangleRec(rect, new Color(51, 153, 225, 20));
    }
}