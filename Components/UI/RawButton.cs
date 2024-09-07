using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class RawButton : ImageComponent
{
    public Color RawColor { get; set; } = Color.White;
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

    public override void Start()
    {
        IsClickable = true;
    }

    public override void Draw()
    {
        base.Draw();
        if(ActiveImage.Id <= 0)
        {
            if(CornerRoundness > 0)
            {
                Raylib.DrawRectangleRounded(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), _cornerRoundness, 0, RawColor);
            } else
            {
                Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), RawColor);
            }
        }
    }
}