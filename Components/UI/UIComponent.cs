using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualBasic;
using Raylib_cs;

namespace Vortex;

public enum EAnchorLocation
{
    ANCHOR_None = 0,
    ANCHOR_TopLeft = 1,
    ANCHOR_TopCenter = 2,
    ANCHOR_TopRight = 3,
    ANCHOR_MiddleLeft = 4,
    ANCHOR_MiddleCenter = 5,
    ANCHOR_MiddleRight = 6,
    ANCHOR_BottomLeft = 7,
    ANCHOR_BottomCenter = 8,
    ANCHOR_BottomRight = 9
}

public enum EOriginLocation
{
    ORIGIN_None = 0,
    ORIGIN_TopLeft = 1,
    ORIGIN_TopCenter = 2,
    ORIGIN_TopRight = 3,
    ORIGIN_MiddleLeft = 4,
    ORIGIN_MiddleCenter = 5,
    ORIGIN_MiddleRight = 6,
    ORIGIN_BottomLeft = 7,
    ORIGIN_BottomCenter = 8,
    ORIGIN_BottomRight = 9
}

public enum EStretchType
{
    STRETCH_None = 0,
    STRETCH_Width = 1,
    STRETCH_Height = 2,
    STRETCH_Full = 3,
}

public class UIComponent : Component
{
    protected EAnchorLocation _anchor;
    public int Anchor 
    {
        get => (int)_anchor;
        set => _anchor = (EAnchorLocation)value;
    }
    protected EOriginLocation _origin;
    public int Origin
    {
        get => (int)_origin;
        set => _origin = (EOriginLocation)value;
    }

    protected EStretchType _stretch;
    public int Stretch
    {
        get => (int)_stretch;
        set => _stretch = (EStretchType)value;
    }

    public TransformComponent OwnerTransform {get; protected set;}

    public float Width {get; set;}
    public float Height { get; set; }
    public float PaddingLeft { get; set; } = 0f;
    public float PaddingTop { get; set; } = 0f;
    public int ZIndex = 0;
    protected Vector2 _offset = Vector2.Zero;
    public Vector2 Offset
    {
        get => _offset;
        set 
        {
            _offset = value;
            SetAnchor(_anchor);
            SetOrigin(_origin);
        }
    }

    private bool _drawDebugRect = false;
    public bool DrawDebugRect 
    {
        get => _drawDebugRect;
        set 
        {
            _drawDebugRect = value;
        }
    }


    public bool IsMouseOver { get; private set; } = false;
    public bool IsClickable = false;                    // Flag if the component is clickable

    public Action OnClick;
    public Action OnMouseEnter;
    public Action OnMouseExit;

    public override void Start()
    {
        base.Start();
        if(Owner != null)
            OwnerTransform = Owner.Transform;

        if(Owner != null)
            Owner.IsCameraRelated = false;

        SetAnchor(_anchor);
        SetOrigin(_origin);

        // Subscribe to the event when the window is resized
        Game.WindowSettings.WindowResizeEvent += (int width, int height) => 
        {
            SetAnchor(_anchor);
            SetOrigin(_origin);
        };
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
        SetStretch(_stretch);
        _anchor = anchor;
        if(OwnerTransform == null)
            return;

        
        switch(_anchor)
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
        _origin = origin;
        SetAnchor(_anchor);
    }

    public void SetOriginAndAnchor(EOriginLocation origin, EAnchorLocation anchor)
    {
        SetStretch(_stretch);
        _origin = origin;
        SetAnchor(anchor);
    }

    public virtual Vector2 GetOrigin()
    {
        SetStretch(_stretch);
        switch(_origin)
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

    public void SetStretch(EStretchType type)
    {
        _stretch = type;

        if(_stretch == EStretchType.STRETCH_Width)
        {
            if(Owner.Parent == null)
            {
                OwnerTransform.Scale = new Vector2
                {
                    X = Game.WindowSettings.WindowWidth / this.Width,
                    Y = OwnerTransform.Scale.Y,
                };
            } else 
            {
                var uiComp = Owner.Parent.GetComponent<UIComponent>();                  // Get reference to the UI component
                if(uiComp != null)
                {
                    OwnerTransform.Scale = new Vector2
                    {
                        X = uiComp.Width / this.Width,
                        Y = uiComp.Height
                    };
                }
            }
            return;
        } else if(_stretch == EStretchType.STRETCH_Height)
        {
            if(Owner.Parent == null)
            {
                OwnerTransform.Scale = new Vector2
                {
                    X = OwnerTransform.Scale.X,
                    Y = Game.WindowSettings.WindowHeight / this.Height
                };
            } else 
            {
                var uiComp = Owner.Parent.GetComponent<UIComponent>();
                if(uiComp != null)
                {
                    OwnerTransform.Scale = new Vector2
                    {
                        X = uiComp.Width,
                        Y = uiComp.Height / this.Height
                    };
                }
            }
        } else if(_stretch == EStretchType.STRETCH_Full)
        {
            if(Owner.Parent == null)
            {
                OwnerTransform.Scale = new Vector2
                {
                    X = Game.WindowSettings.WindowWidth / this.Width,
                    Y = Game.WindowSettings.WindowHeight / this.Height
                };
            } else 
            {
                var uiComp = Owner.Parent.GetComponent<UIComponent>();
                if(uiComp != null)
                {
                    OwnerTransform.Scale = new Vector2
                    {
                        X = uiComp.Width / this.Width,
                        Y = uiComp.Height
                    };
                }
            }
        }

    }

    public UIComponent GetParentUIComp()
    {
        return Owner.Parent.GetComponent<UIComponent>();
    }

    public void DetectMouseEnterAndExit()
    {
        if(Input.IsMouseButtonDown(EMouseButton.MOUSE_Left))
            if(Owner.Name == "File")
                Debug.Print("Gello, World", EPrintMessageType.PRINT_Log);

        var mousePos = Input.GetMousePosition(false);
        var compLeft = OwnerTransform.Position.X;
        var compRight = compLeft + this.Width;
        var compTop = OwnerTransform.Position.Y;
        var compBottom = compTop + this.Height;

        if(mousePos.X >= compLeft && mousePos.X < compRight)
        {
            if(mousePos.Y >= compTop && mousePos.Y < compBottom)
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

    public EAnchorLocation GetAnchorLocation() => _anchor;
    public EOriginLocation GetOriginLocation() => _origin;
    public EStretchType GetStretchType() => _stretch;
}