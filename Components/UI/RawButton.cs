using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class RawButton : UIComponent
{
    public Color NormalColor { get; set; } = Color.RayWhite;
    public Color HoverColor { get; set; } = Color.LightGray;
    public Color DisabledColor { get; set; } = Color.Gray;

    private Color _currentColor;                        // Reference to the current color

    private float _cornerRoundness = 0f;
    public float CornerRoundness
    {
        get => _cornerRoundness;
        set 
        {
            var clamped = Raymath.Clamp(value, 0, 1);
            _cornerRoundness = clamped;
        }
    }

    private bool _useTextAsSize = false;
    public bool UseTextAsSize
    {
        get => _useTextAsSize;
        set 
        {
            if(_buttonTextComp != null)
            {
                _useTextAsSize = value;
                if(_useTextAsSize)
                {
                    _buttonTextComp.CalculateTextSize();
                    UpdateSize();
                }
            }
        }
    }

    private TextComponent? _buttonTextComp;                      // Store reference to the buttons text component
    private string? ButtonTextCompId { get; set; }

    public override void Start()
    {
        base.Start();
        IsClickable = true;
        GetButtonTextComponent();
        UpdateSize();

        _currentColor = NormalColor;
        OnMouseEnter = () => 
        {
            if(IsClickable)
                _currentColor = HoverColor;
        };

        OnMouseExit = () => 
        {
            if(IsClickable)
                _currentColor = NormalColor;
        };
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        if(!IsClickable)
            _currentColor = DisabledColor;
    }

    public override void Draw()
    {
        base.Draw();

        if(CornerRoundness > 0)
        {
            Raylib.DrawRectangleRounded(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), _cornerRoundness, 0, _currentColor);
        } else
        {
            Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), _currentColor);
        }
        
    }

    public void UpdateSize()
    {
        if(UseTextAsSize)
        {
            if(_buttonTextComp != null)
            {
                this.Width = _buttonTextComp.Width;
                this.Height = _buttonTextComp.Height;
            }
            SetOriginAndAnchor(_origin, _anchor);
        }
    }

    private void GetButtonTextComponent()
    {
        if(!string.IsNullOrEmpty(ButtonTextCompId))
        {
            _buttonTextComp = (TextComponent)Component.FindComponentById(ButtonTextCompId);
            return;
        }

        if(_buttonTextComp == null)
            _buttonTextComp = Owner.GetComponentInChild<TextComponent>();
    }
}