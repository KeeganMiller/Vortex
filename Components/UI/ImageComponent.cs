using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex;

public class ImageComponent : UIComponent
{
    public SpriteData NormalImage { get; set; }
    public SpriteData HoverImage { get; set; }
    public Texture2D NormalImageRef => NormalImage.Texture;
    public Texture2D HoverImageRef => HoverImage.Texture;

    public Texture2D ActiveImage { get; private set; }
    public Color Tint = Color.White;
    public Vector2 ImageRotationOrigin = Vector2.Zero;

    public bool IsValid => ActiveImage.Id > 0;
    public override void Start()
    {
        base.Start();
        OnMouseEnter += () => {
            if(HoverImage.IsValid)
                SetActiveImage(HoverImage.Texture);
        };
        OnMouseExit += () => {
            if(NormalImage.IsValid)
                SetActiveImage(NormalImage.Texture);
        };

        OwnerTransform.ScaleUpdateEvent += UpdateImageSize;

        if(NormalImage.IsValid)
            SetActiveImage(NormalImage.Texture);

        OnMouseEnter += () => 
        {
            if(IsClickable)
                Raylib.SetMouseCursor(MouseCursor.PointingHand);
        };

        OnMouseExit += () => 
        {
            if(IsClickable)
                Raylib.SetMouseCursor(MouseCursor.Default);
        };

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

    /// <summary>
    /// Handles setting the size of the image
    /// </summary>
    private void UpdateImageSize()
    {
        if(ActiveImage.Id > 0)
        {
            this.Width = ActiveImage.Width * OwnerTransform.Scale.X;
            this.Height = ActiveImage.Height * OwnerTransform.Scale.Y;
        }
    }

    public override void Draw()
    {
        base.Draw();

        // Make sure the image is valid
        if(ActiveImage.Id > 0)
        {
            var source = new Rectangle(0.0f, 0.0f, ActiveImage.Width, ActiveImage.Height);              // Create the source rect
            var dest = new Rectangle(Owner.Transform.Position, new Vector2(Width, Height));                 // Create the destination rect
            // Draw Texture
            Raylib.DrawTexturePro(ActiveImage, source, dest, ImageRotationOrigin, Owner.Transform.Rotation, Tint);
        }
    }

    private void SetActiveImage(Texture2D image)
    {
        ActiveImage = image;
        UpdateImageSize();
    }
}
