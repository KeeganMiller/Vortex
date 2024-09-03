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

        OnMouseEnter += () => { Raylib.SetMouseCursor(MouseCursor.PointingHand); };
        OnMouseExit += () => { Raylib.SetMouseCursor(MouseCursor.Default); };
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(IsClickable && IsMouseOver)
        {
            if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
                OnClick?.Invoke();
        }
    }

    public override void Draw()
    {
        base.Draw();

        if(ActiveImage.Id > 0)
        {
            var source = new Rectangle(0.0f, 0.0f, ActiveImage.Width, ActiveImage.Height);
            var dest = new Rectangle(Owner.Transform.Position, new Vector2(Width, Height));
            Raylib.DrawTexturePro(ActiveImage, source, dest, Vector2.Zero, Owner.Transform.Rotation, Tint);
        }
    }

    private void SetActiveImage(Texture2D image)
    {
        ActiveImage = image;
        this.Width = image.Width * OwnerTransform.Scale.X;
        this.Height = image.Height * OwnerTransform.Scale.Y;
    }
}
