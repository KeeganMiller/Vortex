using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public class SpriteComponent : Component
{
    public Texture2D Sprite;
    protected TransformComponent _parentTransform;
    public bool IsSpriteValid => Sprite.Id > 0;

    // == Drawing Properties == //
    public Vector2 FramePosition;
    public Vector2 FrameSize;
    public Vector2 Origin;
    public Color Tint = Color.White;
    public bool ScaleWithScreen = false;                        // Use the size of the screen and the default size of the texture

    protected Rectangle _sourceRect;
    protected Rectangle _destRect;

    public SpriteComponent(string name = "SpriteComponent") : base(name)
    {

    }

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        _parentTransform = Owner.Transform;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        if (FrameSize == Vector2.Zero)
        {
            if (IsSpriteValid)
            {
                FrameSize = new Vector2(Sprite.Width, Sprite.Height);
            }
        }
        
        if(IsSpriteValid)
        {
            _sourceRect = new Rectangle(FramePosition, FrameSize);
            Vector2 spriteScale = _parentTransform.Scale;
            if(ScaleWithScreen)
            {
                var scaleX = (float)Game.WindowSettings.WindowWidth / (float)Sprite.Width;
                var scaleY = (float)Game.WindowSettings.WindowHeight / (float)Sprite.Height;
                var scale = Math.Min(scaleX, scaleY);
                spriteScale = new Vector2(scale, scale);
            }

            _destRect = new Rectangle(_parentTransform.Position, FrameSize * spriteScale);
        }
    }

    public override void Draw()
    {
        base.Draw();

        if (IsSpriteValid)
        {
            Raylib.DrawTexturePro(Sprite, _sourceRect, _destRect, Origin, _parentTransform.Rotation, Tint);
        }
    }
}