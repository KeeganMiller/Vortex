using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex.UI;

public class ImageComponent : UIComponent
{
    public Texture2D NormalImage;
    public Texture2D HoverImage;

    public Texture2D ActiveImage { get; private set; }
    public Color Tint = Color.White;

    public bool IsClickable = false;

    public Action OnClick;

    public override void Start()
    {
        base.Start();
        OnMouseEnter += () => {
            if(HoverImage.Id > 0)
                SetActiveImage(HoverImage);
        };
        OnMouseExit += () => {
            if(NormalImage.Id > 0)
                SetActiveImage(NormalImage);
        };

        SetActiveImage(NormalImage);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(IsClickable && IsMouseOver && Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
        {
            OnClick?.Invoke();
        }
    }

    public override void Draw()
    {
        base.Draw();

        if(ActiveImage.Id > 0)
        {
            var source = new Rectangle(0.0f, 0.0f, this.Width, this.Height);
            var dest = new Rectangle(Owner.Transform.Position, new Vector2(this.Width * Owner.Transform.Scale.X, this.Height * Owner.Transform.Scale.Y));
            Raylib.DrawTexturePro(ActiveImage, source, dest, GetOrigin(), Owner.Transform.Rotation, Tint);
            //Raylib.DrawRectangle((int)_ownerTransform.Position.X - (int)GetOrigin().X, (int)_ownerTransform.Position.Y - (int)GetOrigin().Y, (int)this.Width, (int)this.Height, Color.Green);
        }
    }

    private void SetActiveImage(Texture2D image)
    {
        ActiveImage = image;
        this.Width = image.Width;
        this.Height = image.Height;
    }
}
