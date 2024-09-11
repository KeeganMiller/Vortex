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
    public Vector2 FramePosition;
    public Vector2 FrameSize;
    public Vector2 Origin;
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
            Raylib.DrawTexturePro(SpriteRef, _sourceRect, _destRect, Origin, _parentTransform.Rotation, Tint);
        }
    }
}