using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex.UI;

public class ImageComponent : UIComponent
{
    private Texture2D _imageRef;
    public Texture2D Image
    {
        get => _imageRef;
        set 
        {
            _imageRef = value;
            this.Width = _imageRef.Width;
            this.Height = _imageRef.Height;
        }
    }

    public bool IsImageValid => _imageRef.Id > 0;

    public Color Tint = Color.White;

    public override void Draw()
    {
        base.Draw();

        if(IsImageValid)
        {
            var source = new Rectangle(0.0f, 0.0f, this.Width, this.Height);
            var dest = new Rectangle(Owner.Transform.Position, new Vector2(this.Width * Owner.Transform.Scale.X, this.Height * Owner.Transform.Scale.Y));
            Raylib.DrawTexturePro(_imageRef, source, dest, GetOrigin(), Owner.Transform.Rotation, Tint);
        }
    }
}
