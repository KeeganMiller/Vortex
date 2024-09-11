using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class SpriteComponent : Component
{
    public SpriteData Sprite { get; set; }
    public Texture2D SpriteRef => Sprite.Texture;
    protected TransformComponent _parentTransform;
    public bool IsSpriteValid => Sprite.IsValid;

    // == Drawing Properties == //
    public Vector2 FramePosition { get; set; }
    public Vector2 FrameSize { get; set; }
    public EOriginLocation Origin { get; set; }
    public Color Tint = Color.White;
    public bool ScaleWithScreen = false;                        // Use the size of the screen and the default size of the texture
    public int ZIndex = 0;

    protected Rectangle _sourceRect;
    protected Rectangle _destRect;

    public SpriteComponent(string name = "SpriteComponent") : base(name)
    {

    }

    public override void Update(float dt)
    {
        base.Update(dt);
        if (FrameSize == Vector2.Zero)
        {
            if (IsSpriteValid)
            {
                FrameSize = new Vector2(SpriteRef.Width, SpriteRef.Height);
            }
        }
        
        if(IsSpriteValid)
        {
            _sourceRect = new Rectangle(FramePosition, FrameSize);
            _destRect = new Rectangle(Owner.Transform.Position, FrameSize * Owner.Transform.Scale);
        }
    }

    public override void Draw()
    {
        base.Draw();

        if (IsSpriteValid)
        {
            Raylib.DrawTexturePro(SpriteRef, _sourceRect, _destRect, GetOrigin(), _parentTransform.Rotation, Tint);
        }
    }

    private Vector2 GetOrigin()
    {
        if(Sprite.IsValid && Origin != EOriginLocation.ORIGIN_None && Origin != EOriginLocation.ORIGIN_TopLeft)
        {
            switch(Origin)
            {
                case EOriginLocation.ORIGIN_TopCenter:
                    return new Vector2(SpriteRef.Width / 2, 0);
                case EOriginLocation.ORIGIN_TopRight:
                    return new Vector2(SpriteRef.Width, 0);
                case EOriginLocation.ORIGIN_MiddleLeft:
                    return new Vector2(0, SpriteRef.Height / 2);
                case EOriginLocation.ORIGIN_MiddleCenter:
                    return new Vector2(SpriteRef.Width / 2, SpriteRef.Height / 2);
                case EOriginLocation.ORIGIN_MiddleRight:
                    return new Vector2(SpriteRef.Width, SpriteRef.Height / 2);
                case EOriginLocation.ORIGIN_BottomLeft:
                    return new Vector2(0, SpriteRef.Height);
                case EOriginLocation.ORIGIN_BottomCenter:
                    return new Vector2(SpriteRef.Width / 2, SpriteRef.Height);
                case EOriginLocation.ORIGIN_BottomRight:
                    return new Vector2(SpriteRef.Width, SpriteRef.Height);
            }
        }

        return Vector2.Zero;
    }
}